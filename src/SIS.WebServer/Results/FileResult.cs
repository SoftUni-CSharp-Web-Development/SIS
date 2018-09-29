using SIS.HTTP;
using SIS.HTTP.Responses;

namespace SIS.WebServer.Results
{
    public class FileResult : HttpResponse
    {
        public FileResult(byte[] content)
        {
            this.Headers.Add(new HttpHeader("Content-Length", content.Length.ToString()));
            this.Headers.Add(new HttpHeader("Content-Disposition", "inline"));
            this.Content = content;
        }
    }
}
