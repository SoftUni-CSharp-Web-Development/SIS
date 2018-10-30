using System;
using System.Text;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;

namespace SIS.WebServer.Results
{
    public class InternalServerErrorResult : HttpResponse
    {
        private const string DefaultErrorHeading = "<h1>Internal Server Error occured, see details</h1>";

        public InternalServerErrorResult(string content)
            : base(HttpResponseStatusCode.InternalServerError)
        {
            content = DefaultErrorHeading + Environment.NewLine + content;
            this.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html"));
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
