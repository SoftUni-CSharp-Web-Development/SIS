using CakesWebApp.Controllers;
using SIS.HTTP.Enums;
using SIS.MvcFramework;
using SIS.WebServer.Routing;

namespace CakesWebApp
{
    public class Startup : IMvcApplication
    {
        public void Configure(ServerRoutingTable routing)
        {
            // {controller}/{action}/{id}
            routing.Routes[HttpRequestMethod.Get]["/"] = request =>
                new HomeController { Request = request }.Index();
            routing.Routes[HttpRequestMethod.Get]["/register"] = request =>
                new AccountController { Request = request }.Register();
            routing.Routes[HttpRequestMethod.Get]["/login"] = request =>
                new AccountController { Request = request }.Login();
            routing.Routes[HttpRequestMethod.Post]["/register"] = request =>
                new AccountController { Request = request }.DoRegister();
            routing.Routes[HttpRequestMethod.Post]["/login"] = request =>
                new AccountController { Request = request }.DoLogin();
            routing.Routes[HttpRequestMethod.Get]["/hello"] = request =>
                new HomeController { Request = request }.HelloUser();
            routing.Routes[HttpRequestMethod.Get]["/logout"] = request =>
                new AccountController { Request = request }.Logout();
            routing.Routes[HttpRequestMethod.Get]["/cakes/add"] = request =>
                new CakesController { Request = request }.AddCakes();
            routing.Routes[HttpRequestMethod.Post]["/cakes/add"] = request =>
                new CakesController { Request = request }.DoAddCakes();
            // cakes/view?id=1
            routing.Routes[HttpRequestMethod.Get]["/cakes/view"] = request =>
                new CakesController { Request = request }.ById();
        }

        public void ConfigureServices()
        {
            // TODO: Implement IoC/DI container
        }
    }
}
