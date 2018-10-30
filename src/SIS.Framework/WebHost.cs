using SIS.Framework.Api;
using SIS.Framework.Routers;
using SIS.Framework.Routers.Contracts;
using SIS.Framework.Services;
using SIS.WebServer;
using SIS.WebServer.Api;

namespace SIS.Framework
{
    public static class WebHost
    {
        private const int HostingPort = 8000; 

        public static void Start(IMvcApplication application)
        {
            IDependencyContainer container = new DependencyContainer();
            application.ConfigureServices(container);

            IMvcRouter controllerRouter = new ControllerRouter(container);
            IResourceRouter resourceRouter = new ResourceRouter();
            ICustomRouter customRouter = new CustomRouter();

            IHttpRequestHandler httpRequestHandlingContext 
                = new HttpRequestHandlingContext(controllerRouter, resourceRouter, customRouter);

            application.Configure();

            Server server = new Server(HostingPort, httpRequestHandlingContext);
            server.Run();
        }
    }
}
