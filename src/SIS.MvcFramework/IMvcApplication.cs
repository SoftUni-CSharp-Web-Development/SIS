using SIS.MvcFramework.Services;
using SIS.WebServer.Routing;

namespace SIS.MvcFramework
{
    public interface IMvcApplication
    {
        MvcFrameworkSettings Configure();

        void ConfigureServices(IServiceCollection collection);
    }
}
