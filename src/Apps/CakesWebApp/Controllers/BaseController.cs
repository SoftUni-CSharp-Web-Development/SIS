using CakesWebApp.Data;
using SIS.MvcFramework;

namespace CakesWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            this.Db = new CakesDbContext();
        }

        protected CakesDbContext Db { get; }
    }
}
