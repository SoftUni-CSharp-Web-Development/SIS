using SIS.HTTP.Responses;
using SIS.MvcFramework;

namespace CakesWebApp.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("/")]
        public IHttpResponse Index()
        {
            return this.View("Index");
        }

        [HttpGet("/hello")]
        public IHttpResponse HelloUser()
        {
            return this.View("HelloUser", new HelloUserViewModel { Username = this.User.Username });
        }
    }

    public class HelloUserViewModel
    {
        public string Username { get; set; }
    }
}
