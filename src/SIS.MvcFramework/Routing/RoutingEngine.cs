using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Services;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;

namespace SIS.MvcFramework.Routing
{
    public class RoutingEngine
    {
        public void RegisterRoutes(
            ServerRoutingTable routingTable,
            IMvcApplication application, 
            MvcFrameworkSettings settings,
            IServiceCollection serviceCollection)
        {
            RegisterStaticFiles(routingTable, settings);
            RegisterActions(routingTable, application, settings, serviceCollection);
            RegisterDefaultRoute(routingTable);
        }

        private static void RegisterStaticFiles(
            ServerRoutingTable routingTable,
            MvcFrameworkSettings settings)
        {
            var path = settings.WwwrootPath;
            if (!Directory.Exists(path))
            {
                return;
            }

            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var url = file.Replace("\\", "/").Replace(settings.WwwrootPath, string.Empty);
                routingTable.Add(HttpRequestMethod.Get, url, (request) =>
                {
                    var content = File.ReadAllBytes(file);
                    var contentType = "text/plain";
                    if (file.EndsWith(".css"))
                    {
                        contentType = "text/css";
                    }
                    else if (file.EndsWith(".js"))
                    {
                        contentType = "application/javascript";
                    }
                    else if (file.EndsWith(".bmp"))
                    {
                        contentType = "image/bmp";
                    }
                    else if (file.EndsWith(".png"))
                    {
                        contentType = "image/png";
                    }
                    else if (file.EndsWith(".jpg") || file.EndsWith(".jpeg"))
                    {
                        contentType = "image/jpeg";
                    }
                    else if (file.EndsWith(".ico"))
                    {
                        contentType = "image/x-icon";
                    }

                    return new TextResult(content, HttpResponseStatusCode.Ok, contentType);
                });
                Console.WriteLine($"Content registered: {file} => {HttpRequestMethod.Get} => {url}");
            }
        }

        private static void RegisterDefaultRoute(ServerRoutingTable routingTable)
        {
            if (!routingTable.Contains(HttpRequestMethod.Get, "/")
                && routingTable.Contains(HttpRequestMethod.Get, "/Home/Index"))
            {
                routingTable.Add(HttpRequestMethod.Get, "/", (request) =>
                    routingTable.Get(HttpRequestMethod.Get, "/Home/Index")(request));

                Console.WriteLine($"Route registered: reuse /Home/Index => {HttpRequestMethod.Get} => /");
            }
        }

        private static void RegisterActions(
            ServerRoutingTable routingTable, 
            IMvcApplication application,
            MvcFrameworkSettings settings,
            IServiceCollection serviceCollection)
        {
            var userCookieService = serviceCollection.CreateInstance<IUserCookieService>();
            var controllers = application.GetType().Assembly.GetTypes()
                .Where(myType => myType.IsClass
                                 && !myType.IsAbstract
                                 && myType.IsSubclassOf(typeof(Controller)));
            foreach (var controller in controllers)
            {
                var getMethods = controller
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var methodInfo in getMethods)
                {
                    var httpAttribute = (HttpAttribute) methodInfo.GetCustomAttributes(true)
                        .FirstOrDefault(ca => ca.GetType().IsSubclassOf(typeof(HttpAttribute)));
                    
                    var method = HttpRequestMethod.Get;
                    string path = null;
                    if (httpAttribute != null)
                    {
                        method = httpAttribute.Method;
                        path = httpAttribute.Path;
                    }

                    if (path == null)
                    {
                        var controllerName = controller.Name;
                        if (controllerName.EndsWith("Controller"))
                        {
                            controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
                        }

                        var actionName = methodInfo.Name;

                        path = $"/{controllerName}/{actionName}";
                    }
                    else if (!path.StartsWith("/"))
                    {
                        path = "/" + path;
                    }

                    var authorizeAttribute = methodInfo.GetCustomAttributes(true)
                        .FirstOrDefault(ca => ca.GetType() == typeof(AuthorizeAttribute)) as AuthorizeAttribute;
                    routingTable.Add(method, path,
                        (request) =>
                        {
                            if (authorizeAttribute != null)
                            {
                                var userData = Controller.GetUserData(request.Cookies, userCookieService);
                                if (userData == null || !userData.IsLoggedIn
                                    || (authorizeAttribute.RoleName != null
                                        && authorizeAttribute.RoleName != userData.Role))
                                {
                                    var response = new HttpResponse();
                                    response.Headers.Add(new HttpHeader(HttpHeader.Location, settings.LoginPageUrl));
                                    response.StatusCode = HttpResponseStatusCode.SeeOther; // TODO: Found better?
                                    return response;
                                }
                            }

                            return ExecuteAction(controller, methodInfo, request, serviceCollection);
                        });
                    Console.WriteLine($"Route registered: {controller.Name}.{methodInfo.Name} => {method} => {path}");
                }
            }
        }

        private static IHttpResponse ExecuteAction(Type controllerType,
            MethodInfo methodInfo, IHttpRequest request,
            IServiceCollection serviceCollection)
        {
            var controllerInstance = serviceCollection.CreateInstance(controllerType) as Controller;
            if (controllerInstance == null)
            {
                return new TextResult("Controller not found.",
                    HttpResponseStatusCode.InternalServerError);
            }

            controllerInstance.Request = request;
            controllerInstance.ViewEngine = new ViewEngine.ViewEngine(); // TODO: use serviceCollection
            controllerInstance.UserCookieService = serviceCollection.CreateInstance<IUserCookieService>();
            
            var actionParameterObjects = GetActionParameterObjects(methodInfo, request, serviceCollection);
            var httpResponse = methodInfo.Invoke(controllerInstance, actionParameterObjects.ToArray()) as IHttpResponse;
            return httpResponse;
        }

        private static List<object> GetActionParameterObjects(MethodInfo methodInfo, IHttpRequest request,
            IServiceCollection serviceCollection)
        {
            var actionParameters = methodInfo.GetParameters();
            var actionParameterObjects = new List<object>();
            foreach (var actionParameter in actionParameters)
            {
                // TODO: Improve this check
                if (actionParameter.ParameterType.IsValueType ||
                    Type.GetTypeCode(actionParameter.ParameterType) == TypeCode.String)
                {
                    var stringValue = GetRequestData(request, actionParameter.Name);
                    actionParameterObjects.Add(ObjectMapper.TryParse(stringValue, actionParameter.ParameterType));
                }
                else
                {
                    var instance = serviceCollection.CreateInstance(actionParameter.ParameterType);
                    var properties = actionParameter.ParameterType.GetProperties();
                    foreach (var propertyInfo in properties)
                    {
                        // TODO: Support IEnumerable
                        var stringValue = GetRequestData(request, propertyInfo.Name);

                        // Convert.ChangeType()
                        var value = ObjectMapper.TryParse(stringValue, propertyInfo.PropertyType);

                        propertyInfo.SetMethod.Invoke(instance, new object[] { value });
                    }

                    actionParameterObjects.Add(instance);
                }
            }

            return actionParameterObjects;
        }

        private static string GetRequestData(IHttpRequest request, string key)
        {
            key = key.ToLower();
            string stringValue = null;
            if (request.FormData.Any(x => x.Key.ToLower() == key))
            {
                stringValue = request.FormData.First(x => x.Key.ToLower() == key).Value.ToString().UrlDecode();
            }
            else if (request.QueryData.Any(x => x.Key.ToLower() == key))
            {
                stringValue = request.QueryData.First(x => x.Key.ToLower() == key).Value.ToString().UrlDecode();
            }

            return stringValue;
        }
    }
}
