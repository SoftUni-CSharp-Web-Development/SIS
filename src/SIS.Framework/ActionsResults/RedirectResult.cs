using SIS.Framework.ActionsResults.Contracts;

namespace SIS.Framework.ActionsResults
{
    public class RedirectResult : IRedirectable
    {
        public RedirectResult(string redirectUrl)
        {
            this.RedirectUrl = redirectUrl;
        }

        public string RedirectUrl { get; }

        public string Invoke() =>
            this.RedirectUrl;
    }
}
