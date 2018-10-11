using SIS.Framework.ActionsResults.Base;

namespace SIS.Framework.ActionsResults.Contracts
{
    public interface IViewable : IActionResult
    {
        IRenderable View { get; set; }
    }
}
