using System;
using CakesWebApp.Controllers;
using CakesWebApp.Data;
using Microsoft.EntityFrameworkCore;
using SIS.HTTP.Enums;
using SIS.MvcFramework;
using SIS.MvcFramework.Logger;
using SIS.MvcFramework.Services;
using SIS.WebServer.Routing;

namespace CakesWebApp
{
    public class Startup : IMvcApplication
    {
        public void Configure()
        {

        }
        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<IUserCookieService, UserCookieService>();
            collection.AddService<ILogger>(() => new FileLogger($"log.txt"));
            collection.AddService<IUserManager, UserManager>();

            /*
             * In ASP.NET Core
            collection.AddTransient<IUserCookieService, UserCookieService>();
            collection.AddScoped<IUserCookieService, UserCookieService>();
            collection.AddSingleton<IUserCookieService, UserCookieService>();
             */
        }
    }
}
