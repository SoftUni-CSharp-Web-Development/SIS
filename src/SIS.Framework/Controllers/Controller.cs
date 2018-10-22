using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using SIS.Framework.ActionResults;
using SIS.Framework.Models;
using SIS.Framework.Security;
using SIS.Framework.Utilities;
using SIS.Framework.Views;
using SIS.HTTP.Requests;

namespace SIS.Framework.Controllers
{
    public abstract class Controller
    {
        protected Controller()
        {
            this.Model = new ViewModel();
        }
        public Model ModelState { get; } = new Model();

        protected ViewModel Model { get; }

        public IHttpRequest Request { get; set; }
            
        private ViewEngine ViewEngine { get;  } = new ViewEngine();

        protected IViewable View([CallerMemberName] string caller = "",ViewModel model = null)
        {
            var controllerName = ControllerUtilities.GetControllerName(this);
            string viewContent = null;

            try
            {
                viewContent = this.ViewEngine.GetViewContent(controllerName, actionName: caller);
            }
            catch (FileNotFoundException e)
            {
                this.Model.Data["Error"] = e.Message;

                viewContent = this.ViewEngine.GetErrorContent();
            }

            if (model != null)
            {
                model = this.PopulateModelData(model);
            }
            
            //Extended from documentation
            string renderedContent = model == null ? this.ViewEngine.RenderHtml(viewContent, this.Model.Data) :
                    this.ViewEngine.RenderHtml(viewContent, model.Data);
            

            return new ViewResult(new View(renderedContent));
        }

        private ViewModel PopulateModelData(ViewModel model)
        {
            var properties = model.GetType().GetProperties(System.Reflection.BindingFlags.Public
                                                           | System.Reflection.BindingFlags.Instance
                                                           | System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var propertyInfo in properties)
            {
                model[propertyInfo.Name] = propertyInfo.GetValue(model);
            }

            return model;
            
        }

        protected IRedirectable RedirectToAction(string redirectUrl) => new RedirectResult(redirectUrl);


        protected void SignIn(IIdentity auth)
        {
            if (!this.Request.Session.ContainsParameter("auth"))
            {

                this.Request.Session.AddParameter("auth", auth);
            }
        }

        protected void SignOut()
        {
            this.Request.Session.ClearParameters();
        }

        public IIdentity Identity => (IIdentity) this.Request.Session.GetParameter("auth");
    }
}