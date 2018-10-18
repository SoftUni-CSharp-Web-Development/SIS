using System.Collections.Generic;
using System.IO;
using System.Linq;
using SIS.Framework.ActionsResults.Contracts;
using SIS.HTTP.Common;

namespace SIS.Framework.Views
{
    public class View : IRenderable
    {
        private const string RenderBodyConstant = "@RenderBody()";

        private readonly string fullyQualifiedTemplateName;

        private readonly IDictionary<string, object> viewData;

        public View(string fullyQualifiedTemplateName, IDictionary<string, object> viewData)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
            this.viewData = viewData;
        }

        private string ReadFile()
        {
            if (!File.Exists(this.fullyQualifiedTemplateName))
            {
                throw new FileNotFoundException($"View does not exist at {fullyQualifiedTemplateName}");
            }

            return File.ReadAllText(this.fullyQualifiedTemplateName);
        }

        public string Render()
        {
            var fullHtml = this.ReadFile();
            var renderedHtml = this.RenderHtml(fullHtml);

            var layoutWithView = this.AddViewToLayout(renderedHtml);

            return layoutWithView;
        }

        private string AddViewToLayout(string renderedHtml)
        {
            var layoutViewPath = MvcContext.Get.RootDirectoryRelativePath +
                GlobalConstants.DirectorySeparator +
                MvcContext.Get.ViewsFolderName +
                GlobalConstants.DirectorySeparator +
                MvcContext.Get.LayoutViewName +
                GlobalConstants.HtmlFileExtension;

            if (!File.Exists(layoutViewPath))
            {
                throw new FileNotFoundException($"View does not exist at {fullyQualifiedTemplateName}");
            }

            var layoutViewContent = File.ReadAllText(layoutViewPath);
            var layoutWithView = layoutViewContent.Replace(RenderBodyConstant, renderedHtml);

            return layoutWithView;

        }

        private string RenderHtml(string fullHtml)
        {
            if (this.viewData.Any())
            {
                foreach (var viewDataKey in this.viewData.Keys)
                {
                    var dynamicDataPlaceholder = $"{{{{{viewDataKey}}}}}";
                    if (fullHtml.Contains(dynamicDataPlaceholder))
                    {
                        fullHtml = fullHtml.Replace(
                            dynamicDataPlaceholder,
                            this.viewData[viewDataKey].ToString());
                    }
                }
            }

            return fullHtml;
        }
    }
}
