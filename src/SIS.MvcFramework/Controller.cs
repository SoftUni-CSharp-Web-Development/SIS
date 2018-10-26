using System;
using System.Text;
using SIS.HTTP.Cookies;
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
        protected Controller()
        {
            this.Response = new HttpResponse {StatusCode = HttpResponseStatusCode.Ok};
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        public IViewEngine ViewEngine { get; set; }

        public IUserCookieService UserCookieService { get; internal set; }

        public static MvcUserInfo GetUserData(
            IHttpCookieCollection cookieCollection,
            IUserCookieService cookieService)
        {
            if (!cookieCollection.ContainsCookie(".auth-cakes"))
            {
                return new MvcUserInfo();
            }

            var cookie = cookieCollection.GetCookie(".auth-cakes");
            var cookieContent = cookie.Value;

            try
            {
                var userName = cookieService.GetUserData(cookieContent);
                return userName;
            }
            catch (Exception)
            {
                return new MvcUserInfo();
            }
        }

        protected MvcUserInfo User => GetUserData(this.Request.Cookies, this.UserCookieService);

        protected IHttpResponse View(string viewName = null, string layoutName = "_Layout")
        {
            return this.View(viewName, (object)null, layoutName);
        }
        
        protected IHttpResponse View<T>(T model = null, string layoutName = "_Layout")
            where T : class
        {
            return this.View(null, model, layoutName);
        }

        protected IHttpResponse View<T>(
            string viewName = null,
            T model = null,
            string layoutName = "_Layout")
            where T : class
        {
            if (viewName == null)
            {
                viewName = this.Request.Path.Trim('/', '\\');
                if (string.IsNullOrWhiteSpace(viewName))
                {
                    viewName = "Home/Index";
                }
            }

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
            var viewModel = new ErrorViewModel { Error = errorMessage };
            var allContent = this.GetViewContent("Error", viewModel);
            this.PrepareHtmlResult(allContent);
            this.Response.StatusCode = HttpResponseStatusCode.BadRequest;
            return this.Response;
        }

        protected IHttpResponse BadRequestErrorWithView(string errorMessage)
        {
            return this.BadRequestErrorWithView(errorMessage, (object)null);
        }

        protected IHttpResponse BadRequestErrorWithView<T>(string errorMessage, T model, string layoutName = "_Layout")
        {
            var errorContent = this.GetViewContent("Error", new ErrorViewModel { Error = errorMessage }, null);

            var viewName = this.Request.Path.Trim('/', '\\');
            if (string.IsNullOrWhiteSpace(viewName))
            {
                viewName = "Home/Index";
            }

            var viewContent = this.GetViewContent(viewName, model, null);
            var allViewContent = errorContent + Environment.NewLine + viewContent;
            var errorAndViewContent = this.ViewEngine.GetHtml(viewName, allViewContent, model, this.User);

            var layoutFileContent = System.IO.File.ReadAllText($"Views/{layoutName}.html");
            var allContent = layoutFileContent.Replace("@RenderBody()", errorAndViewContent);
            var layoutContent = this.ViewEngine.GetHtml("_Layout", allContent, model, this.User);

            this.PrepareHtmlResult(layoutContent);
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

            if (layoutName != null)
            {
                var layoutFileContent = System.IO.File.ReadAllText($"Views/{layoutName}.html");
                var allContent = layoutFileContent.Replace("@RenderBody()", content);
                var layoutContent = this.ViewEngine.GetHtml("_Layout", allContent, model, this.User);
                return layoutContent;
            }

            return content;
        }

        private void PrepareHtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
