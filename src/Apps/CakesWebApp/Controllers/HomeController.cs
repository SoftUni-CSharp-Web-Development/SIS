using System.Collections.Generic;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.MvcFramework;
using SIS.MvcFramework.Services;

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
            return this.View("HelloUser", new HelloUserViewModel { Username = this.User });
        }
    }

    public class HelloUserViewModel
    {
        public string Username { get; set; }
    }
}
