using System;
using SIS.HTTP.Responses;
using System.Linq;
using CakesWebApp.Models;
using CakesWebApp.ViewModels.Account;
using SIS.HTTP.Cookies;
using SIS.MvcFramework;
using SIS.MvcFramework.Services;

namespace CakesWebApp.Controllers
{
    // account/register
    public class AccountController : BaseController
    {
        private readonly IHashService hashService;

        public AccountController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        [HttpGet("/account/register")]
        public IHttpResponse Register()
        {
            return this.View("Register");
        }

        [HttpPost("/account/register")]
        public IHttpResponse DoRegister(DoRegisterInputModel model)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(model.Username) || model.Username.Trim().Length < 4)
            {
                return this.BadRequestError("Please provide valid username with length of 4 or more characters.");
            }

            if (this.Db.Users.Any(x => x.Username == model.Username.Trim()))
            {
                return this.BadRequestError("User with the same name already exists.");
            }

            if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
            {
                return this.BadRequestError("Please provide password of length 6 or more.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return this.BadRequestError("Passwords do not match.");
            }

            // Hash password
            var hashedPassword = this.hashService.Hash(model.Password);

            // Create user
            var user = new User
            {
                Name = model.Username.Trim(),
                Username = model.Username.Trim(),
                Password = hashedPassword,
            };
            this.Db.Users.Add(user);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO: Log error
                return this.ServerError(e.Message);
            }

            // TODO: Login

            // Redirect
            return this.Redirect("/");
        }

        [HttpGet("/account/login")]
        public IHttpResponse Login()
        {
            return this.View("Login");
        }

        [HttpPost("/account/login")]
        public IHttpResponse DoLogin(DoLoginInputModel model)
        {
            var hashedPassword = this.hashService.Hash(model.Password);

            var user = this.Db.Users.FirstOrDefault(x => 
                x.Username == model.Username.Trim() &&
                x.Password == hashedPassword);

            if (user == null)
            {
                return this.BadRequestError("Invalid username or password.");
            }

            var mvcUser = new MvcUserInfo { Username = user.Username };
            var cookieContent = this.UserCookieService.GetUserCookie(mvcUser);

            var cookie = new HttpCookie(".auth-cakes", cookieContent, 7) { HttpOnly = true };
            this.Response.Cookies.Add(cookie);
            return this.Redirect("/");
        }

        [HttpGet("/account/logout")]
        public IHttpResponse Logout()
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cakes"))
            {
                return this.Redirect("/");
            }

            var cookie = this.Request.Cookies.GetCookie(".auth-cakes");
            cookie.Delete();
            this.Response.Cookies.Add(cookie);
            return this.Redirect("/");
        }
    }
}
