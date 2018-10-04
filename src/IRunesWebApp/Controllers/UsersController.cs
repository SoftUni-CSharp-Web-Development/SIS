using System;
using System.Linq;
using IRunesWebApp.Models;
using Services;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace IRunesWebApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly HashService hashService;

        public UsersController()
        {
            this.hashService = new HashService();
        }

        public IHttpResponse Login(IHttpRequest request) => this.View();

        public IHttpResponse PostLogin(IHttpRequest request)
        {
            var username = request.FormData["username"].ToString();
            var password = request.FormData["password"].ToString();

            var hashedPassword = this.hashService.Hash(password);

            var user = this.Context.Users
                .FirstOrDefault(u => u.Username == username &&
                    u.HashedPassword == hashedPassword);

            if (user == null)
            {
                return new RedirectResult("/login");
            }

            var response = new RedirectResult("/home/index");
            this.SignInUser(username, response, request);
            return response;
        }

        public IHttpResponse Register(IHttpRequest request) => this.View();

        public IHttpResponse PostRegister(IHttpRequest request)
        {
            var userName = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var confirmPassword = request.FormData["confirmPassword"].ToString();

            // Validate
            //if (string.IsNullOrWhiteSpace(userName) || userName.Length < 4)
            //{
            //    return new BadRequestResult("Please provide valid username with length of 4 or more characters.");
            //}

            //if (this.Context.Users.Any(x => x.Username == userName))
            //{
            //    return new BadRequestResult("User with the same name already exists.");
            //}

            //if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            //{
            //    return new BadRequestResult("Please provide password of length 6 or more.");
            //}

            if (password != confirmPassword)
            {
                return new BadRequestResult(
                    "Passwords do not match.",
                    HttpResponseStatusCode.SeeOther);
            }

            // Hash password
            var hashedPassword = this.hashService.Hash(password);

            // Create user
            var user = new User
            {
                Username = userName,
                HashedPassword = hashedPassword,
            };
            this.Context.Users.Add(user);

            try
            {
                this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO: Log error
                return new BadRequestResult(
                    e.Message,
                    HttpResponseStatusCode.InternalServerError);
            }

            var response = new RedirectResult("/");
            this.SignInUser(userName, response, request);

            // Redirect
            return response;
        }


    }
}
