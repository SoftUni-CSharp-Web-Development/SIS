namespace SIS.Framework.ActionResults
{
    public interface IRedirectable : IActionResult
    {
        string RedirectUrl { get; }
    }
}
