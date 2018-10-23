using SIS.HTTP.Responses;
using SIS.MvcFramework;

namespace MishMashWebApp.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("/Home/Index")]
        public IHttpResponse Index()
        {
            if (this.User != null)
            {
                // TODO: prepare view model
                return this.View("Home/LoggedInIndex");
            }
            else
            {
                return this.View("Home/Index");
            }
        }

        [HttpGet("/")]
        public IHttpResponse RootIndex()
        {
            return this.Index();
        }
    }
}
