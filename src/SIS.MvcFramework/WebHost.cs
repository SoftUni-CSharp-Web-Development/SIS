using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Logger;
using SIS.MvcFramework.Services;
using SIS.WebServer;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;

namespace SIS.MvcFramework
{
    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            var dependencyContainer = new ServiceCollection();
            dependencyContainer.AddService<IHashService, HashService>();
            dependencyContainer.AddService<IUserCookieService, UserCookieService>();
            dependencyContainer.AddService<ILogger>(() => new FileLogger($"log.txt"));

            application.ConfigureServices(dependencyContainer);

            var serverRoutingTable = new ServerRoutingTable();
            RegisterRoutes(serverRoutingTable, application, dependencyContainer);

            application.Configure();

            var server = new Server(80, serverRoutingTable);
            server.Run();
        }

        private static void RegisterRoutes(ServerRoutingTable routingTable, 
            IMvcApplication application, IServiceCollection serviceCollection)
        {
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
                    var httpAttribute = (HttpAttribute)methodInfo.GetCustomAttributes(true)
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

                    routingTable.Add(method, path,
                        (request) => ExecuteAction(controller, methodInfo, request, serviceCollection));
                    Console.WriteLine($"Route registered: {controller.Name}.{methodInfo.Name} => {method} => {path}");
                }
            }

            if (!routingTable.Routes[HttpRequestMethod.Get].ContainsKey("/")
                && routingTable.Routes[HttpRequestMethod.Get].ContainsKey("/Home/Index"))
            {
                routingTable.Routes[HttpRequestMethod.Get]["/"] = (request) =>
                    routingTable.Routes[HttpRequestMethod.Get]["/Home/Index"](request);

                Console.WriteLine($"Route registered: reuse /Home/Index => {HttpRequestMethod.Get} => /");
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

                        propertyInfo.SetMethod.Invoke(instance, new object[] {value});
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
