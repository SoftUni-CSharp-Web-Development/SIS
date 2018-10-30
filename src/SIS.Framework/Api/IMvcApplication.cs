using SIS.Framework.Services;

namespace SIS.Framework.Api
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IDependencyContainer dependencyContainer);
    }
}

