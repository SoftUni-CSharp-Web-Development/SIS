using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SIS.Framework.Views
{
    public class ViewEngine
    {
        private const string ViewPathPrefix = @"..\..\..";

        private const string DisplayTemplateSuffix = "DisplayTemplate";

        private const string LayoutViewName = "_Layout";

        private const string ErrorViewName = "_Error";

        private const string ViewExtension = "html";

        private const string ModelCollectionViewParameterPattern = @"\@Model\.Collection\.(\w+)\((.+)\)";

        private string ViewsFolderPath => $@"{ViewPathPrefix}\{MvcContext.Get.ViewsFolder}\";

        private string ViewsSharedFolderPath => $@"{this.ViewsFolderPath}Shared\";

        private string ViewsDisplayTemplateFolderPath => $@"{this.ViewsSharedFolderPath}\DisplayTemplates\";

        private string FormatLayoutViewPath()
            => $@"{this.ViewsSharedFolderPath}{LayoutViewName}.{ViewExtension}";

        private string FormatErrorViewPath()
            => $@"{this.ViewsSharedFolderPath}{ErrorViewName}.{ViewExtension}";

        private string FormatViewPath(string controllerName, string actionName)
            => $@"{this.ViewsFolderPath}\{controllerName}\{actionName}.{ViewExtension}";

        private string FormatDisplayTemplatePath(string objectName)
            => $@"{this.ViewsDisplayTemplateFolderPath}{objectName}{DisplayTemplateSuffix}.{ViewExtension}";

        private string ReadLayoutHtml(string layoutViewPath)
        {
            if (!File.Exists(layoutViewPath))
            {
                throw new FileNotFoundException($"Layout View does not exist.");
            }

            return File.ReadAllText(layoutViewPath);
        }

        private string ReadErrorHtml(string errorViewPath)
        {
            if (!File.Exists(errorViewPath))
            {
                throw new FileNotFoundException($"Error View does not exist.");
            }

            return File.ReadAllText(errorViewPath);
        }

        private string ReadViewHtml(string viewPath)
        {
            if (!File.Exists(viewPath))
            {
                throw new FileNotFoundException($"View does not exist at {viewPath}");
            }

            return File.ReadAllText(viewPath);
        }

        private string RenderObject(object viewObject, string displayTemplate)
        {
            foreach (var property in viewObject.GetType().GetProperties())
            {
                displayTemplate =
                    this.RenderViewData(displayTemplate
                        , property.GetValue(viewObject)
                        , property.Name);
            }

            return displayTemplate;
        }

        private string RenderViewData(string template
            , object viewObject
            , string viewObjectName = null)
        {
            if (viewObject != null
                && viewObject.GetType() != typeof(string)
                && viewObject is IEnumerable enumerable
                && Regex.IsMatch(template, ModelCollectionViewParameterPattern))
            {
                Match collectionMatch =
                    Regex.Matches(template, ModelCollectionViewParameterPattern)
                    .First(cm => cm.Groups[1].Value == viewObjectName);

                string fullMatch = collectionMatch.Groups[0].Value;
                string itemPattern = collectionMatch.Groups[2].Value;

                string result = string.Empty;

                foreach (var subObject in enumerable)
                {
                    result += itemPattern
                        .Replace("@Item", this.RenderViewData(template, subObject));
                }

                return template.Replace(fullMatch, result);
            }

            if (viewObject != null
                && !viewObject.GetType().IsPrimitive
                && viewObject.GetType() != typeof(string))
            {
                if (File.Exists(this.FormatDisplayTemplatePath(viewObject.GetType().Name)))
                {
                    string renderedObject = this.RenderObject(viewObject,
                        File.ReadAllText(this.FormatDisplayTemplatePath(viewObject.GetType().Name)));

                    return viewObjectName != null
                           ? template.Replace($"@Model.{viewObjectName}", renderedObject)
                           : renderedObject;
                }
            }

            return viewObjectName != null
                ? template.Replace($"@Model.{viewObjectName}", viewObject?.ToString())
                : viewObject?.ToString();
        }

        public string GetErrorContent()
            => this.ReadLayoutHtml(this.FormatLayoutViewPath())
                .Replace("@RenderError()", this.ReadErrorHtml(this.FormatErrorViewPath()));

        public string GetViewContent(string controllerName, string actionName)
            => this.ReadLayoutHtml(this.FormatLayoutViewPath())
                .Replace("@RenderBody()", this.ReadViewHtml(this.FormatViewPath(controllerName, actionName)));

        public string RenderHtml(string fullHtmlContent, IDictionary<string, object> viewData)
        {
            string renderedHtml = fullHtmlContent;

            if (viewData.Count > 0)
            {
                foreach (var parameter in viewData)
                {
                    renderedHtml = 
                        this.RenderViewData(renderedHtml, parameter.Value, parameter.Key);
                }
            }

            if (viewData.ContainsKey("Error"))
            {
                renderedHtml = renderedHtml.Replace("@Error", viewData["Error"].ToString());
            }

            return renderedHtml;
        }
    }
}
