using System;
using System.Collections.Generic;
using System.Text;
using SIS.MvcFramework;
using SIS.MvcFramework.Services;

namespace ChushkaWebApp
{
    public class Startup : IMvcApplication
    {
        public MvcFrameworkSettings Configure()
        {
            return new MvcFrameworkSettings();
        }

        public void ConfigureServices(IServiceCollection collection)
        {
        }
    }
}
