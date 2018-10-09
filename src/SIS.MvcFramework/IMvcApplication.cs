using SIS.WebServer.Routing;

namespace SIS.MvcFramework
{
    public interface IMvcApplication
    {
        void Configure(ServerRoutingTable routing);

        void ConfigureServices();
    }
}
