using SIS.Framework.ActionResults;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Method;
using TORSHIA.App.Models.Binding;
using TORSHIA.Domain;
using TORSHIA.Services.Contracts;

namespace TORSHIA.App.Controllers
{
    public class UsersController : BaseController
    {
        public readonly IUsersService userService;

        public UsersController(IUsersService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Login(LoginUserBindingModel bindingModel)
        {
            if (this.ModelState.IsValid != true)
            {
                return this.View();
            }

            User userFromDb = this.userService.GetUserByUsername(bindingModel.Username);

            if (userFromDb == null || userFromDb.Password != bindingModel.Password)
            {
                return this.View();
            }

            this.SignIn(userFromDb);
            return this.RedirectToAction("/Home/Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterUserBindingModel bindingModel)
        {
            if (this.ModelState.IsValid != true
                && bindingModel.Password != bindingModel.ConfirmPassword)
            {
                return this.View();
            }

            this.userService.CreateUser(bindingModel);
            return this.RedirectToAction("/Users/Login");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            this.SignOut();

            return this.RedirectToAction("/Home/Index");
        }
    }
}
