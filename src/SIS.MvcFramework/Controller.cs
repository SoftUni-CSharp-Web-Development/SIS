﻿using System.Collections.Generic;
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
        protected Controller()
        {
            this.Response = new HttpResponse {StatusCode = HttpResponseStatusCode.Ok};
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        public IViewEngine ViewEngine { get; set; }

        public IUserCookieService UserCookieService { get; internal set; }

        protected string User
        {
            get
            {
                if (!this.Request.Cookies.ContainsCookie(".auth-cakes"))
                {
                    return null;
                }

                var cookie = this.Request.Cookies.GetCookie(".auth-cakes");
                var cookieContent = cookie.Value;
                var userName = this.UserCookieService.GetUserData(cookieContent);
                return userName;
            }
        }
        
        protected IHttpResponse View(string viewName, string folderName = null)
        {
            var allContent = this.GetViewContent(viewName, (object)null, folderName);
            this.PrepareHtmlResult(allContent);
            return this.Response;
        }
        
        protected IHttpResponse View<T>(string viewName, T model = null, string folderName = null)
            where T : class
        {
            var allContent = this.GetViewContent(viewName, model, folderName);
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
            var viewModel = new BadRequestViewModel()
            {
                ErrorMessage = errorMessage
            };
            var allContent = this.GetViewContent("Error", viewModel);
            this.PrepareHtmlResult(allContent);
            this.Response.StatusCode = HttpResponseStatusCode.BadRequest;
            return this.Response;
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            var viewModel = new ServerErrorViewModel()
            {
                ErrorMessage = errorMessage
            };
            var allContent = this.GetViewContent("Error", errorMessage);
            this.PrepareHtmlResult(allContent);
            this.Response.StatusCode = HttpResponseStatusCode.InternalServerError;
            return this.Response;
        }

        private string GetViewContent<T>(string viewName, T model, string folderName = null)
        {
            if (folderName != null)
            {
                folderName = folderName + "/";
            }
            var content = this.ViewEngine.GetHtml(viewName,
                System.IO.File.ReadAllText("Views/" + folderName + viewName + ".html"), model);

            var layoutFileContent = System.IO.File.ReadAllText("Views/_Layout.html");
            var allContent = layoutFileContent.Replace("@RenderBody()", content);
            var layoutContent = this.ViewEngine.GetHtml("_Layout", allContent, model);
            return layoutContent;
        }

        private void PrepareHtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
        }
    }

    public class ServerErrorViewModel
    {
        public string ErrorMessage { get; set; }
    }

    public class BadRequestViewModel
    {
        public string ErrorMessage { get; set; }
    }
}
