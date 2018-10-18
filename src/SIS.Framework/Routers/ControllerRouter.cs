using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using SIS.Framework.ActionsResults.Base;
using SIS.Framework.ActionsResults.Contracts;
using SIS.Framework.Attributes.Methods;
using SIS.Framework.Attributes.Methods.Base;
using SIS.Framework.Controllers;
using SIS.Framework.Services.Contracts;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Extensions;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Api.Contracts;
using SIS.WebServer.Results;

namespace SIS.Framework.Routers
{
    public class ControllerRouter : IHttpHandler
    {
        private readonly IDependencyContainer dependencyContainer;

        public ControllerRouter(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            var controllerName = string.Empty;
            var actionName = string.Empty;
            var requestMethod = request.RequestMethod.ToString();

            if (request.Path == "/")
            {
                controllerName = "Home";
                actionName = "Index";
            }
            else
            {
                var requestUrlSplit = request.Path.Split(
                    "/",
                    StringSplitOptions.RemoveEmptyEntries);
                
                controllerName = requestUrlSplit[0].Capitalize();
                actionName = requestUrlSplit[1].Capitalize();
            }

            //Controller
            var controller = this.GetController(controllerName);

            //Action
            var action = this.GetAction(requestMethod, controller, actionName);

            if (controller == null || action == null)
            {
                throw new NullReferenceException();
            }
            controller.Request = request;
            object[] actionParameters = this.MapActionParameters(action, request, controller);

            var actionResult = InvokeAction(controller, action, actionParameters);

            return this.PrepareResponse(actionResult);
        }

        private Controller GetController(string controllerName)
        {
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return null;
            }

            var fullyQualifiedControllerName = string.Format("{0}.{1}.{2}{3}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ControllersFolder,
                controllerName,
                MvcContext.Get.ControllerSuffix);

            var controllerType = Type.GetType(fullyQualifiedControllerName);
            var controller = (Controller)this.dependencyContainer.CreateInstance(controllerType);
            return controller;
        }

        private MethodInfo GetAction(
            string requestMethod,
            Controller controller,
            string actionName)
        {
            var actions = this
                .GetSuitableMethods(controller, actionName)
                .ToList();

            if (!actions.Any())
            {
                return null;
            }

            foreach (var action in actions)
            {
                var httpMethodAttributes = action
                    .GetCustomAttributes()
                    .Where(ca => ca is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>()
                    .ToList();

                if (!httpMethodAttributes.Any() &&
                    requestMethod.ToLower() == "get")
                {
                    return action;
                }

                foreach (var httpMethodAttribute in httpMethodAttributes)
                {
                    if (httpMethodAttribute.IsValid(requestMethod))
                    {
                        return action;
                    }
                }
            }

            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(
            Controller controller,
            string actionName)
        {
            if (controller == null)
            {
                return new MethodInfo[0];
            }

            return controller
                .GetType()
                .GetMethods()
                .Where(mi => mi.Name.ToLower() == actionName.ToLower());
        }

        private IHttpResponse PrepareResponse(IActionResult actionResult)
        {          
            string invokationResult = actionResult.Invoke();

            if (actionResult is IViewable)
            {
                return new HtmlResult(invokationResult, HttpResponseStatusCode.Ok);
            }

            if (actionResult is IRedirectable)
            {
                return new RedirectResult(invokationResult);
            }

            throw new InvalidOperationException("Type of result is not supported");
        }

        private static IActionResult InvokeAction(
            Controller controller,
            MethodInfo action,
            object[] actionParameters)
        {
            return (IActionResult)action.Invoke(controller, actionParameters);
        }

        private object[] MapActionParameters(
            MethodInfo action,
            IHttpRequest request,
            Controller controller)
        {
            var actionParameteres = action.GetParameters();
            object[] mappedActionParameters = new object[actionParameteres.Length];
            for (int i = 0; i < actionParameteres.Length; i++)
            {
                var actionParameter = actionParameteres[i];

                if (actionParameter.ParameterType.IsPrimitive ||
                    actionParameter.ParameterType == typeof(string))
                {
                    var mappedActionParameter = new object();
                    mappedActionParameter = this.ProcessPrimitiveParameter(actionParameter, request);
                    if (mappedActionParameter == null)
                    {
                        break;
                    }
                }
                else
                {
                    var bindingModel = this.ProcessesBindingModelParameter(actionParameter, request);
                    controller.ModelState.IsValid = this.IsValid(
                        bindingModel,
                        actionParameter.ParameterType);
                    mappedActionParameters[i] = bindingModel;
                }
                
            }

            return mappedActionParameters;


        }

        private bool? IsValid(object bindingModel, Type bindingModelType)
        {
            var properties = bindingModelType.GetProperties();

            foreach (var property in properties)
            { 
                var propertyValidationAttributes = property
                    .GetCustomAttributes()
                    .Where(ca => ca is ValidationAttribute)
                    .Cast<ValidationAttribute>()
                    .ToList();

                foreach (var validationAttribute in propertyValidationAttributes)
                {
                    var propertyValue = property.GetValue(bindingModel);

                    if (!validationAttribute.IsValid(propertyValue))
                    {
                        // password -> "error msg"
                        // property -> error
                        return false;
                    }
                }
            }

            return true;
        }

        private object ProcessPrimitiveParameter(
            ParameterInfo actionParameter,
            IHttpRequest request)
        {
            var value = this.GetParameterFromRequestData(request, actionParameter.Name);
            if (value == null)
            {
                return value;
            }
            return Convert.ChangeType(value, actionParameter.ParameterType);
        }

        private object ProcessesBindingModelParameter(
            ParameterInfo actionParameter,
            IHttpRequest request)
        {
            var bindingModelType = actionParameter.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);

            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var bindingModelProperty in bindingModelProperties)
            {
                try
                {
                    var value = this.GetParameterFromRequestData(
                        request,
                        bindingModelProperty.Name.ToLower());

                    bindingModelProperty.SetValue(
                        bindingModelInstance,
                        Convert.ChangeType(value, bindingModelProperty.PropertyType));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The property {bindingModelProperty.Name} could not be mapped");
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object GetParameterFromRequestData(
            IHttpRequest request,
            string actionParameterName)
        {
            if (request.QueryData.ContainsKey(actionParameterName))
            {
                return request.QueryData[actionParameterName];
            }

            if (request.FormData.ContainsKey(actionParameterName))
            {
                return request.FormData[actionParameterName];
            }

            return null;
        }
    }
}
