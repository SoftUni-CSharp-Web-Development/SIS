using System.Linq;
using System.Text;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Extensions;
using SIS.HTTP.Headers;

namespace SIS.HTTP.Responses
{
    public class HttpResponse : IHttpResponse
    {
        public HttpResponse() { }

        public HttpResponse(HttpResponseStatusCode statusCode)
        {
            CoreValidator.ThrowIfNull(statusCode, nameof(statusCode));

            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.Content = new byte[0];
            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set;  }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public byte[] Content { get; set; }
        
        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));
            this.Headers.Add(header);
        }

        public void AddCookie(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));
            this.Cookies.Add(cookie);
        }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(this.ToString()).Concat(this.Content).ToArray();
        }

        public override string ToString()
        {
            var response = new StringBuilder();
           
            response.AppendLine($"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetResponseLine()}")
                .AppendLine(this.Headers.ToString());

            if (this.Cookies.HasCookies())
            {
                var cookiesKeyValuePairs = this.Cookies.ToString().Split(", ");

                foreach (var cookie in cookiesKeyValuePairs)
                {
                    response.AppendLine($"{GlobalConstants.CookieResponseHeaderName}: {cookie}"); 
                }
            }

            response.AppendLine();

            return response.ToString();
        }
    }
}
