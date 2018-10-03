using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public class HomeController : BaseController
    {
        public IHttpResponse Index(IHttpRequest request)
        {
            return this.View("Index");
        }

        public IHttpResponse HelloUser(IHttpRequest request)
        {
            return new HtmlResult($"<h1>Hello, {this.GetUsername(request)}</h1>", HttpResponseStatusCode.Ok);
        }
    }
}
