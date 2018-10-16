namespace SIS.MvcFramework.ViewEngine
{
    public class ViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewName, string viewCode, T model)
        {
            var csharpMethodBody = this.GenerateCSharpMethodBody(viewCode);
            var viewCodeAsCSharpCode = @"
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
namespace MyAppViews
{
    public class " + viewName + @"View : IView<" + typeof(T).FullName + @">
    {
        public string GetHtml(" + typeof(T).FullName + @" model)
        {
            StringBuilder html = new StringBuilder();

            " + csharpMethodBody + @"

            return html.ToString();
        }
    }
}";
            var instanceOfViewClass = this.GetInstance(viewCodeAsCSharpCode) as IView<T>;
            var html = instanceOfViewClass.GetHtml(model);
            return html;
        }

        private object GetInstance(string cSharpCode)
        {
            throw new System.NotImplementedException();
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            return string.Empty;
        }
    }
}
