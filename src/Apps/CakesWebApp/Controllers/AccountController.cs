using System;
using CakesWebApp.Data;
using Microsoft.EntityFrameworkCore.Internal;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
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
        private readonly IUserManager userManager;

        public AccountController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet("/account/register")]
        public IHttpResponse Register()
        {
            return this.View("Register");
        }

        [HttpPost("/account/register")]
        public IHttpResponse DoRegister(DoRegisterInputModel model)
        {
            return this.userManager.RegisterUser(model);
        }

        [HttpGet("/account/login")]
        public IHttpResponse Login()
        {
            return this.View("Login");
        }

        [HttpPost("/account/login")]
        public IHttpResponse DoLogin(DoLoginInputModel model)
        {
            return this.userManager.LoginUser(model);
        }

        [HttpGet("/account/logout")]
        public IHttpResponse Logout()
        {
            return this.userManager.LogOut(this.Request);
        }
    }
}
