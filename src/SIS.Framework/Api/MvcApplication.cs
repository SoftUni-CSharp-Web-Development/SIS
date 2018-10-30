using SIS.Framework.Services;

namespace SIS.Framework.Api
{
    public class MvcApplication : IMvcApplication
    {
        public virtual void Configure() { }

        public virtual void ConfigureServices(IDependencyContainer dependencyContainer) { }
    }
}
