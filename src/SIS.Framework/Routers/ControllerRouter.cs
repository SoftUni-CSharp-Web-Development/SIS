using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using SIS.Framework.ActionResults;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Method;
using SIS.Framework.Controllers;
using SIS.Framework.Routers.Contracts;
using SIS.Framework.Services;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace SIS.Framework.Routers
{
    public class ControllerRouter : IMvcRouter
    {
        private const string DefaultRoute = "/";

        private const string RequestUrlControllerActionSeparator = "/";

        private const string DefaultControllerName = "Home";

        private const string DefaultControllerActionName = "Index";

        private const string DefaultControllerActionRequestMethod = "GET";

        private const string UnsupportedActionMessage = "The Action result is not supported.";

        private readonly IDependencyContainer dependencyContainer;

        public ControllerRouter(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        private Controller GetController(string controllerName, IHttpRequest request)
        {
            if (controllerName != null)
            {
                string controllerTypeFullName =
                    $"{MvcContext.Get.AssemblyName}." +
                    $"{MvcContext.Get.ControllerFolder}." +
                    $"{controllerName}{MvcContext.Get.ControllersSuffix}, " +
                    $"{MvcContext.Get.AssemblyName}";

                Type controllerType = Type.GetType(controllerTypeFullName);
                Controller controllerInstance = (Controller)this.dependencyContainer.CreateInstance(controllerType);

                if (controllerInstance != null)
                {
                    controllerInstance.Request = request;
                }

                return controllerInstance;
            }

            return null;
        }

        private string[] ExtractControllerAndActionNames(IHttpRequest request)
        {
            string[] result = new string[2];

            if (request.Path == DefaultRoute)
            {
                result[0] = DefaultControllerName;
                result[1] = DefaultControllerActionName;
            }
            else
            {
                var requestUrlSplit = request.Path.Split(
                    RequestUrlControllerActionSeparator
                    , StringSplitOptions.RemoveEmptyEntries);

                result[0] = requestUrlSplit[0];
                result[1] = requestUrlSplit[1];
            }

            return result;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            if (controller == null)
            {
                return new MethodInfo[0];
            }

            return controller
                .GetType()
                .GetMethods()
                .Where(m => m.Name.ToLower() == actionName.ToLower());
        }

        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            foreach (var controllerAction in this.GetSuitableMethods(controller, actionName))
            {
                var controllerActionAttributes = controllerAction
                    .GetCustomAttributes()
                    .Where(a => a is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>()
                    .ToList();

                if (!controllerActionAttributes.Any()
                    && requestMethod.ToUpper() == DefaultControllerActionRequestMethod) return controllerAction;

                foreach (var attribute in controllerActionAttributes)
                {
                    if (attribute.IsValid(requestMethod)) return controllerAction;
                }
            }

            return null;
        }

        private object GetParameterFromRequestData(IHttpRequest httpRequest, string paramName)
        {
            if (httpRequest.QueryData.ContainsKey(paramName)) return httpRequest.QueryData[paramName];
            if (httpRequest.FormData.ContainsKey(paramName)) return httpRequest.FormData[paramName];
            return null;
        }

        private bool IsValidModel(object bindingModel)
        {
            foreach (var property in bindingModel.GetType().GetProperties())
            {
                IEnumerable<ValidationAttribute> validationAttributes
                    = property.GetCustomAttributes()
                        .Where(a => a is ValidationAttribute)
                        .Cast<ValidationAttribute>()
                        .ToList();

                if (validationAttributes.Any(a => !a.IsValid(property.GetValue(bindingModel))))
                {
                    return false;
                }
            }

            return true;
        }

        private object ProcessPrimitiveParameter(ParameterInfo param, IHttpRequest httpRequest)
        {
            object value = this.GetParameterFromRequestData(httpRequest, param.Name);
            return Convert.ChangeType(value, param.ParameterType);
        }

        private object ProcessBindingModelParameter(ParameterInfo param, IHttpRequest httpRequest)
        {
            Type bindingModelType = param.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);
            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var property in bindingModelProperties)
            {
                try
                {
                    object value = this.GetParameterFromRequestData(httpRequest, property.Name);
                    property.SetValue(bindingModelInstance, Convert.ChangeType(value, property.PropertyType));
                }
                catch
                {
                    Console.WriteLine($"The {property.Name} field could not be mapped.");
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object[] MapActionParameters(Controller controller, MethodInfo action, IHttpRequest httpRequest)
        {
            ParameterInfo[] actionParametersInfo = action.GetParameters();
            object[] mappedActionParameters = new object[actionParametersInfo.Length];

            for (int index = 0; index < actionParametersInfo.Length; index++)
            {
                ParameterInfo currentParameterInfo = actionParametersInfo[index];

                if (currentParameterInfo.ParameterType.IsPrimitive
                    || currentParameterInfo.ParameterType == typeof(string))
                {
                    mappedActionParameters[index] = ProcessPrimitiveParameter(currentParameterInfo, httpRequest);
                }
                else
                {
                    object bindingModel = ProcessBindingModelParameter(currentParameterInfo, httpRequest);
                    controller.ModelState.IsValid = this.IsValidModel(bindingModel);
                    mappedActionParameters[index] = bindingModel;
                }
            }

            return mappedActionParameters;
        }

        private bool IsAuthorized(Controller controller, MethodInfo action) 
            => action
                .GetCustomAttributes()
                .Where(a => a is AuthorizeAttribute)
                .Cast<AuthorizeAttribute>()
                .All(a => a.IsAuthorized(controller.Identity));

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
            => (IActionResult)action.Invoke(controller, actionParameters);

        private IHttpResponse PrepareResponse(IActionResult actionResult)
        {
            string invocationResult = actionResult.Invoke();

            if (actionResult is IViewable)
            {
                return new HtmlResult(invocationResult, HttpResponseStatusCode.Ok);
            }

            if (actionResult is IRedirectable)
            {
                return new RedirectResult(invocationResult);
            }

            throw new InvalidOperationException(UnsupportedActionMessage);
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            string[] controllerAndActionNames = this.ExtractControllerAndActionNames(request);

            string controllerName = controllerAndActionNames[0];
            string actionName = controllerAndActionNames[1];

            Controller controller = this.GetController(controllerName, request);
            MethodInfo action = this.GetMethod(request.RequestMethod.ToString(), controller, actionName);

            if (controller == null || action == null)
            {
                return null;
            }

            object[] actionParameters = this.MapActionParameters(controller, action, request);

            if (!this.IsAuthorized(controller, action))
            {
                return new UnauthorizedResult();
            }

            return this.PrepareResponse(this.InvokeAction(controller, action, actionParameters));
        }
    }
}