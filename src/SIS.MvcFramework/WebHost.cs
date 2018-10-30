using System.Globalization;
using SIS.MvcFramework.Logger;
using SIS.MvcFramework.Routing;
using SIS.MvcFramework.Services;
using SIS.WebServer;
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
            var settings = application.Configure();

            var serverRoutingTable = new ServerRoutingTable();
            var routingEngine = new RoutingEngine();
            routingEngine.RegisterRoutes(serverRoutingTable, application, settings, dependencyContainer);

            var server = new Server(settings.PortNumber, serverRoutingTable);
            server.Run();
        }
    }
}
