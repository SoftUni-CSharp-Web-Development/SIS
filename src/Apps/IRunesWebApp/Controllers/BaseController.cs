using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using IRunesWebApp.Data;

using Services;

using SIS.Framework.Controllers;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace IRunesWebApp.Controllers
{
    public abstract class BaseController : Controller
    {       
        private const string RootDirectoryRelativePath = "../../../";

        private const string ControllerDefaultName = "Controller";

        private const string DirectorySeparator = "/";

        private const string ViewsFolderName = "Views";

        private const string HtmlFileExtension = ".html";

        private const string LayoutViewFileName = "_Layout";

        private const string RenderBodyConstant = "@RenderBody()";

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

        protected IHttpResponse ViewMethod([CallerMemberName] string viewName = "")
        {
            var layoutView = RootDirectoryRelativePath +
                ViewsFolderName +
                DirectorySeparator +
                LayoutViewFileName + 
                HtmlFileExtension;

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

            var viewContent = BuildViewContent(filePath);


            var viewLayout = File.ReadAllText(layoutView);
            var view = viewLayout.Replace(RenderBodyConstant, viewContent);

            var response = new HtmlResult(view, HttpResponseStatusCode.Ok);

            return response;
        }

        private string BuildViewContent(string filePath)
        {
            var viewContent = File.ReadAllText(filePath);

            foreach (var viewBagKey in ViewBag.Keys)
            {
                var dynamicDataPlaceholder = $"{{{{{viewBagKey}}}}}";
                if (viewContent.Contains(dynamicDataPlaceholder))
                {
                    viewContent = viewContent.Replace(
                        dynamicDataPlaceholder,
                        this.ViewBag[viewBagKey]);
                }
            }

            return viewContent;
        }
    }
}
