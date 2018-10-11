using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace IRunesWebApp.Controllers
{
    public class HomeController : BaseController
    {        
        public IHttpResponse Index(IHttpRequest request)
        {
            if (this.IsAuthenticated(request))
            {
                var username = request.Session.GetParameter("username");
                this.ViewBag["username"] = username.ToString();

                return this.View("IndexLoggedIn");
            }

            return this.View();
        }
    }
}
