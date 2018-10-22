using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SIS.Framework.ActionResults;

namespace SIS.Framework.Views
{
    public class View : IRenderable
    {
        /*public string Render()
        {
            var fullHtml = this.ReadFile();
            string renderedHtml = this.RenderHtml(fullHtml);
            return fullHtml;
        }

        private string RenderHtml(string fullHtml)
        {
            string renderedHtml = fullHtml;

            if (this.viewData.Any())
            {
                foreach (var parameter in this.viewData)
                {
                    renderedHtml = renderedHtml.Replace($"{{{{{{{parameter.Key}}}}}}}", parameter.Value.ToString());
                }
            }

            return renderedHtml;
        }

        private string ReadFile()
        {
            if (!File.Exists(this.fullyQualifiedTemplateName ))
            {
                throw new FileNotFoundException();
            }
            
            var content = File.ReadAllText(this.fullyQualifiedTemplateName);

            return content;
        }

        private readonly string fullyQualifiedTemplateName;

        private readonly IDictionary<string, object> viewData;

        public View(string fullyQualifiedTemplateName, IDictionary<string,object> viewData)
        {
            this.fullyQualifiedTemplateName = "D:\\SIS-Cake\\SIS\\src\\SIS.Demo\\" + fullyQualifiedTemplateName;
            this.viewData = viewData;
        }
        */

        private readonly string fullHtmlContent;


        public View(string fullHtmlContent, IDictionary<string,object> data)
        {
            this.fullHtmlContent = fullHtmlContent;
        }

        public View(string fullHtmlContent)
        {
            this.fullHtmlContent = fullHtmlContent;
        }

        public string Render() => this.fullHtmlContent;
    }
}