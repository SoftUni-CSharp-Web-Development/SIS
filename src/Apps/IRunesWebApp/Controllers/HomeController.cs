using System;
using Services;
using SIS.Framework.ActionsResults.Base;
using SIS.Framework.Controllers;

namespace IRunesWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHashService hashService;

        public HomeController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        public HomeController()
        {
            
        }

        public IActionResult Index(IndexViewModel model)
        {
            var hashedUsername = this.hashService.Hash(model.Username);
            Console.WriteLine(hashedUsername);
            return this.View();
        }
    }
}
