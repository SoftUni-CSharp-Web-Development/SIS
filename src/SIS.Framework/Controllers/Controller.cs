using System.IO;
using System.Runtime.CompilerServices;
using SIS.Framework.ActionResults;
using SIS.Framework.ActionResults.Implementations;
using SIS.Framework.Models;
using SIS.Framework.Security;
using SIS.Framework.Utilities;
using SIS.Framework.Views;
using SIS.HTTP.Requests;

namespace SIS.Framework.Controllers
{
    public class Controller
    {
        private ViewEngine ViewEngine { get; } = new ViewEngine();

        protected ViewModel Model { get; } = new ViewModel();

        public IHttpRequest Request { get; set; }

        public IIdentity Identity 
            => this.Request.Session.ContainsParameter("auth")
                ? (IIdentity)this.Request.Session.GetParameter("auth")
                : null;

        public Model ModelState { get; } = new Model();

        protected virtual IViewable View([CallerMemberName] string actionName = "")
        {
            string controllerName = ControllerUtilities.GetControllerName(this);
            string viewContent = null;

            try
            {
                viewContent = this.ViewEngine.GetViewContent(controllerName, actionName);
            }
            catch (FileNotFoundException e)
            {
                this.Model.Data["Error"] = e.Message;

                viewContent = this.ViewEngine.GetErrorContent();
            }

            string renderedContent = this.ViewEngine.RenderHtml(viewContent, this.Model.Data);
            return new ViewResult(new View(renderedContent));
        }

        protected IRedirectable RedirectToAction(string redirectUrl)
            => new RedirectResult(redirectUrl);

        protected void SignIn(IIdentity auth)
        {
            this.Request.Session.AddParameter("auth", auth);
        }

        protected void SignOut()
        {
            this.Request.Session.ClearParameters();
        }
    }
}