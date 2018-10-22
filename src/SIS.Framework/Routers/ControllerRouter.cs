using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Services;
using SIS.Framework.ActionResults;
using SIS.Framework.Attributes;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Controllers;
using SIS.Framework.Services;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Api;
using SIS.WebServer.Results;

namespace SIS.Framework.Routers
{
    public class ControllerRouter : IHttpHandler
    {
        private IDependencyContainer _dependencyContainer;

        public ControllerRouter(IDependencyContainer dependencyContainer)
        {
            this._dependencyContainer = dependencyContainer;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            var requestMethod = request.RequestMethod.ToString();
            var tokens = new List<string>();

            if (request.Path.Length > 1)
            {
              tokens = request.Path.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList();
                if (tokens.Last().Contains("?"))
                {
                    var formParams = tokens.Last().Split("?").Skip(1).ToString();
                    if (formParams.Contains("&"))
                    {
                        var cleanFormParams = formParams.Split("&");
                        foreach (var param in cleanFormParams)
                        {
                            var keyValue = param.Split("=");
                            request.FormData.Add(keyValue[0],keyValue[1]);
                        }
                    }
                    else
                    {
                       var key = formParams.Split("=")[0];
                       var value = formParams.Split("=")[1];
                       request.FormData.Add(key, value);
                    }
                }
            }


            //refactor
            if (request.Path.Equals("/"))
            {
                tokens = new List<string> {"Home", "Index"};
            }

            var controllerName = tokens[0]  + "Controller";

            var actionName = tokens[1];

            var controller = this.GetController(controllerName, request);

            var action = this.GetMethod(requestMethod, controller, actionName);

            if (action == null)
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            object[] actionParameters = this.MapActionParameters(action, request,controller);
            
            IActionResult actionResult = this.InvokeAction(controller, action, actionParameters);

            return this.Authorize(controller,action) ?? 
                   this.PrepareResponse(this.InvokeAction(controller,action,actionParameters));
        }

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
        {
            return (IActionResult) action.Invoke(controller, actionParameters);
        }

        private object[] MapActionParameters(MethodInfo action, IHttpRequest request, Controller controller)
        {
            ParameterInfo[] actionParametersInfo = action.GetParameters();
            object[] mappedActionParameters = new object[actionParametersInfo.Length];

            for (int i = 0; i < actionParametersInfo.Length; i++)
            {
                ParameterInfo currentParameterInfo = actionParametersInfo[i];
                if (currentParameterInfo.ParameterType.IsPrimitive || currentParameterInfo.ParameterType == typeof(string))
                {
                    mappedActionParameters[i] = ProcessPrimitiveParameter(currentParameterInfo, request);
                }
                else
                {
                    
                    object bindingModel = ProcessBindingModelParameter(currentParameterInfo, request);
                    
                    controller.ModelState.IsValid = this.IsValidModel(bindingModel);
                    mappedActionParameters[i] = bindingModel;
                }
            }

            return mappedActionParameters;
        }

        private bool IsValidModel(object bindingModel)
        {
            Type modelType = bindingModel.GetType();
            IList<PropertyInfo> propertyInfos = new List<PropertyInfo>(modelType.GetProperties());
            foreach (var propertyInfo in propertyInfos)
            {
                var attributes = propertyInfo
                    .GetCustomAttributes(typeof(ValidationAttribute),false)
                    .Cast<ValidationAttribute>()
                    .ToList();
                
                var value = propertyInfo.GetValue(propertyInfo);

                if (!attributes.All(x => x.IsValid(value)))
                {
                    return false;
                }

                
                
            }

            return true;
        }

        private object ProcessBindingModelParameter(ParameterInfo currentParameterInfo, IHttpRequest request)
        {
            Type bindingModelType = currentParameterInfo.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);
            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var property in bindingModelProperties)
            {
                try
                {
                    object value = this.GetParameterFromRequestData(request, property.Name);
                    property.SetValue(bindingModelInstance, Convert.ChangeType(value,property.PropertyType));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The {property.Name} field could not be mapped.");
                    throw;
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object ProcessPrimitiveParameter(ParameterInfo param, IHttpRequest request)
        {
            object value = this.GetParameterFromRequestData(request, param.Name);
            return Convert.ChangeType(value, param.ParameterType);
        }

        private object GetParameterFromRequestData(IHttpRequest request, string paramName)
        {
            if (request.QueryData.ContainsKey(paramName)) return request.QueryData[paramName];
            if (request.FormData.ContainsKey(paramName)) return request.FormData[paramName];
            return null;
        }

        private Controller GetController(string controllerName, IHttpRequest request)
        {
            if (controllerName == null) return null;

            var controllerTypeName = string.Format("{0}.{1}.{2}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ControllersFolders,
                controllerName);

            var controllerType = Type.GetType(controllerTypeName);

            this._dependencyContainer.RegisterDependency<IHashService, HashService>();

            //var controller = (Controller) Activator.CreateInstance(controllerType);
            var controller = (Controller)_dependencyContainer.CreateInstance(controllerType);


            if (controller != null)
            {
                controller.Request = request;
            }

            return controller;

        }

        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            MethodInfo method = null;

            foreach (var methodInfo in GetSuitableMethods(controller,actionName))
            {
                var attributes = methodInfo.GetCustomAttributes().Where(attr => attr is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>();

                if (!attributes.Any() && requestMethod.ToUpper() == "GET")
                {
                    return methodInfo;
                }

                foreach (var attribute in attributes)
                {
                    if (attribute.IsValid(requestMethod))
                    {
                        return methodInfo;
                    }   
                }
            }

            return method;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            return controller == null ? new MethodInfo[0] : controller
                .GetType()
                .GetMethods()
                .Where(methodInfo => string.Equals(methodInfo.Name, actionName, StringComparison.CurrentCultureIgnoreCase));
        }

        private IHttpResponse PrepareResponse(IActionResult actionResult)
        {
            var invocationResult = actionResult.Invoke();

            if (actionResult is IViewable)
            {
                return new HtmlResult(invocationResult, HttpResponseStatusCode.Ok);
            }

            if (actionResult is IRedirectable)
            {
                return new SIS.WebServer.Results.RedirectResult(invocationResult);
            }

            throw new InvalidOperationException("The view result is not supported");

        }


        private IHttpResponse Authorize(Controller controller, MethodInfo action)
        {
            if (action.GetCustomAttributes()
                .Where(a => a is AuthorizeAttribute)
                .Cast<AuthorizeAttribute>()
                .Any(a => !a.IsAuthorized(controller.Identity)))
            {
                return new UnauthorizedResult();
            }

            return null;
        }

    }
}