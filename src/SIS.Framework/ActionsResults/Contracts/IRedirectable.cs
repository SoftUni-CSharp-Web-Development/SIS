using SIS.Framework.ActionsResults.Base;

namespace SIS.Framework.ActionsResults.Contracts
{
    public interface IRedirectable : IActionResult
    {
        string RedirectUrl { get; }
    }
}
