using ChushkaWebApp.Data;
using SIS.MvcFramework;

namespace ChushkaWebApp.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            this.Db = new ApplicationDbContext();
        }

        public ApplicationDbContext Db { get; }
    }
}
