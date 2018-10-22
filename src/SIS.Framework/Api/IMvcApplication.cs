using SIS.Framework.Services;

namespace SIS.Framework.Api
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureService(IDependencyContainer dependencyContainer);
    }
}