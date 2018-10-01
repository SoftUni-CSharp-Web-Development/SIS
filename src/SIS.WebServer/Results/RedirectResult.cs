using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;

namespace SIS.WebServer.Results
{
    public class RedirectResult : HttpResponse
    {
        public RedirectResult(string location) 
            : base(HttpResponseStatusCode.SeeOther)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.Location, location));
        }
    }
}
