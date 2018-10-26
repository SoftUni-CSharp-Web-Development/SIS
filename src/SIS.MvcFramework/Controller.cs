using System.Collections.Generic;
using System.Text;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Services;
using SIS.MvcFramework.ViewEngine;

namespace SIS.MvcFramework
{
    public abstract class Controller
    {
        protected const string SESSION_KEY = "username";
        protected const string AUTH_COOKIE_KEY = ".auth-cakes";

        protected Controller()
        {
            this.Response = new HttpResponse {StatusCode = HttpResponseStatusCode.Ok};
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        public IViewEngine ViewEngine { get; set; }

        public IUserCookieService UserCookieService { get; internal set; }

        protected UserModel User
        {
            get
            {
                if (!this.Request.Session.ContainsParameter(SESSION_KEY)
                    || !this.Request.Cookies.ContainsCookie(AUTH_COOKIE_KEY))
                {
                    return new UserModel();
                }

                var cookie = this.Request.Cookies.GetCookie(AUTH_COOKIE_KEY);
                var cookieContent = cookie.Value;
                var userName = this.UserCookieService.GetUserData(cookieContent);

                var model = Request.Session.GetParameter(SESSION_KEY);

                var role = model.GetType().GetProperty("Role");
                var roleValue = role.GetValue(model);

                var userModel = new UserModel { Name = userName, Role = roleValue.ToString(), Exist = true };
                return userModel;
            }
        }

        protected IHttpResponse View(string viewName, string layoutName = "_Layout")
        {
            var allContent = this.GetViewContent(viewName, (object)null, layoutName);
            this.PrepareHtmlResult(allContent);
            return this.Response;
        }
        
        protected IHttpResponse View<T>(string viewName, T model = null, string layoutName = "_Layout")
            where T : class
        {
            var allContent = this.GetViewContent(viewName, model, layoutName);
            this.PrepareHtmlResult(allContent);
            return this.Response;
        }

        protected IHttpResponse File(byte[] content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, "inline"));
            this.Response.Content = content;
            return this.Response;
        }

        protected IHttpResponse Redirect(string location)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.Location, location));
            this.Response.StatusCode = HttpResponseStatusCode.SeeOther; // TODO: Found better?
            return this.Response;
        }

        protected IHttpResponse Text(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/plain; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
            return this.Response;
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            var viewModel = new ErrorViewModel {Error = errorMessage};
            var allContent = this.GetViewContent("Error", viewModel);
            this.PrepareHtmlResult(allContent);
            this.Response.StatusCode = HttpResponseStatusCode.BadRequest;
            return this.Response;
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            var viewModel = new ErrorViewModel { Error = errorMessage };
            var allContent = this.GetViewContent("Error", viewModel);
            this.PrepareHtmlResult(allContent);
            this.Response.StatusCode = HttpResponseStatusCode.InternalServerError;
            return this.Response;
        }

        private string GetViewContent<T>(string viewName, T model, string layoutName = "_Layout")
        {
            var content = this.ViewEngine.GetHtml(viewName,
                System.IO.File.ReadAllText("Views/" + viewName + ".html"), model, this.User);

            var layoutFileContent = System.IO.File.ReadAllText($"Views/{layoutName}.html");
            var allContent = layoutFileContent.Replace("@RenderBody()", content);
            var layoutContent = this.ViewEngine.GetHtml("_Layout", allContent, model, this.User);
            return layoutContent;
        }

        private void PrepareHtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
