using SIS.MvcFramework.Services;
using SIS.WebServer.Routing;

namespace SIS.MvcFramework
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IServiceCollection collection);
    }
}
