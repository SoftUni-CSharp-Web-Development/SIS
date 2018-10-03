using System.IO;
using CakesWebApp.Data;
using CakesWebApp.Services;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public abstract class BaseController
    {
        protected BaseController()
        {
            this.Db = new CakesDbContext();
            this.UserCookieService = new UserCookieService();
        }

        protected CakesDbContext Db { get; }


        protected IUserCookieService UserCookieService { get; }

        protected string GetUsername(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth-cakes"))
            {
                return null;
            }

            var cookie = request.Cookies.GetCookie(".auth-cakes");
            var cookieContent = cookie.Value;
            var userName = this.UserCookieService.GetUserData(cookieContent);
            return userName;

        }

        protected IHttpResponse View(string viewName)
        {
            var content = File.ReadAllText("Views/" + viewName + ".html");
            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            return new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.BadRequest);
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            return new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.InternalServerError);
        }
    }
}
