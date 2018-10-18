using System;
using Services;
using SIS.Framework.ActionsResults.Base;
using SIS.Framework.Controllers;

namespace IRunesWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
