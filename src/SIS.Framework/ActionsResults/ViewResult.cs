using System;
using SIS.Framework.ActionsResults.Contracts;

namespace SIS.Framework.ActionsResults
{
    public class ViewResult : IViewable
    {
        public ViewResult(IRenderable view)
        {
            this.View = view;
        }

        public IRenderable View { get; set; }

        public string Invoke() =>
            this.View.Render();
    }
}
