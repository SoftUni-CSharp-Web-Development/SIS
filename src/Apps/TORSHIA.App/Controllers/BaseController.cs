using System.Linq;
using System.Runtime.CompilerServices;
using SIS.Framework.ActionResults;
using SIS.Framework.Controllers;

namespace TORSHIA.App.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override IViewable View([CallerMemberName] string actionName = "")
        {
            this.Model["guestNavbarDisplay"] = "none";
            this.Model["userNavbarDisplay"] = "none";
            this.Model["adminNavbarDisplay"] = "none";

            if (this.Identity == null)
            {
                this.Model["guestNavbarDisplay"] = "flex";
            } else if (this.Identity != null && this.Identity.Roles.Contains("User"))
            {
                this.Model["userNavbarDisplay"] = "flex";
            } else if (this.Identity != null && this.Identity.Roles.Contains("Admin"))
            {
                this.Model["adminNavbarDisplay"] = "flex";
            }

            return base.View(actionName);
        }
    }
}
