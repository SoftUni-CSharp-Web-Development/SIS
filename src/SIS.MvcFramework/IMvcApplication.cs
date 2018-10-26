using SIS.MvcFramework.Services;

namespace SIS.MvcFramework
{
    public interface IMvcApplication
    {
        MvcFrameworkSettings Configure();

        void ConfigureServices(IServiceCollection collection);
    }
}
