using System;
using System.Runtime.CompilerServices;
using SIS.Framework.ActionsResults;
using SIS.Framework.ActionsResults.Contracts;
using SIS.Framework.Models;
using SIS.Framework.Security.Contracts;
using SIS.Framework.Utilities;
using SIS.Framework.Views;
using SIS.HTTP.Requests;

namespace SIS.Framework.Controllers
{
    public abstract class Controller
    {
        protected Controller()
        {
            this.ViewModel = new ViewModel();
        }

        public Model ModelState { get; } = new Model();

        public IHttpRequest Request { get; set; }

        public ViewModel ViewModel { get; set; }

        private ViewEngine ViewEngine { get; } = new ViewEngine();

        protected IViewable View([CallerMemberName] string viewName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);
            string viewContent = null;

            try
            {
                viewContent = this.ViewEngine.GetViewContent(controllerName, viewName);
            }
            catch (Exception e)
            {
                this.ViewModel.Data["error"] = e.Message;

                viewContent = this.ViewEngine.GetErrorContent();
            }

            string renderedHtml = this.ViewEngine.RenderHtml(viewContent, this.ViewModel.Data);
            var view = new View(renderedHtml);

            return new ViewResult(view);
        }

        protected IRedirectable RedirectToAction(string redirectUrl)
            => new RedirectResult(redirectUrl);

        protected void SignIn(IIdentity identity)
        {
            this.Request.Session.AddParameter("auth", identity);
        }

        protected void SignOut()
        {
            this.Request.Session.ClearParameters();
        }

        public IIdentity Identity()
        {
            if (this.Request.Session.ContainsParameter("auth"))
            {
                return (IIdentity)this.Request.Session.GetParameter("auth");
            }

            return null;
        }
    }
}
