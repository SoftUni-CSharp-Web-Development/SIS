using SIS.WebServer;
using SIS.WebServer.Routing;

namespace SIS.MvcFramework
{
    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            application.ConfigureServices();

            var serverRoutingTable = new ServerRoutingTable();
            application.Configure(serverRoutingTable);

            var server = new Server(80, serverRoutingTable);
            server.Run();
        }
    }
}
