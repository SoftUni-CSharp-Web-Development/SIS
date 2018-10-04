using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using IRunesWebApp.Data;
using Services;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace IRunesWebApp.Controllers
{
    public abstract class BaseController
    {       
        private const string RootDirectoryRelativePath = "../../../";

        private const string ControllerDefaultName = "Controller";

        private const string DirectorySeparator = "/";

        private const string ViewsFolderName = "Views";

        private const string HtmlFileExtension = ".html";

        protected IRunesContext Context { get; set; }

        private readonly UserCookieService cookieService;

        protected IDictionary<string, string> ViewBag { get; set; }

        public BaseController()
        {
            this.Context = new IRunesContext();
            this.cookieService = new UserCookieService();
            this.ViewBag = new Dictionary<string, string>();
        }

        public bool IsAuthenticated(IHttpRequest request)
        {
            return request.Session.ContainsParameter("username");
        }

        public void SignInUser(
            string username,
            IHttpResponse response, 
            IHttpRequest request)
        {
            request.Session.AddParameter("username", username);
            var userCookieValue = this.cookieService.GetUserCookie(username);
            response.Cookies.Add(new HttpCookie("IRunes_auth", userCookieValue));
        }

        private string GetCurrentControllerName() =>
            this.GetType().Name.Replace(ControllerDefaultName, string.Empty);

        protected IHttpResponse View([CallerMemberName] string viewName = "")
        {
            string filePath = RootDirectoryRelativePath +
                ViewsFolderName +
                DirectorySeparator +
                this.GetCurrentControllerName() +
                DirectorySeparator +
                viewName +
                HtmlFileExtension;

            if (!File.Exists(filePath))
            {
                return new BadRequestResult(
                    $"View {viewName} not found.",
                    HttpResponseStatusCode.NotFound);
            }

            var fileContent = File.ReadAllText(filePath);

            foreach (var viewBagKey in ViewBag.Keys)
            {
                var dynamicDataPlaceholder = $"{{{{{viewBagKey}}}}}";
                if (fileContent.Contains(dynamicDataPlaceholder))
                {
                    fileContent = fileContent.Replace(
                        dynamicDataPlaceholder,
                        this.ViewBag[viewBagKey]);
                }
            }

            var response = new HtmlResult(fileContent, HttpResponseStatusCode.Ok);

            return response;
        }
    }
}
