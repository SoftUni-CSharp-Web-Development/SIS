using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace SIS.WebServer.Api
{
    public interface IHttpRequestHandler
    {
        IHttpResponse Handle(IHttpRequest request);
    }
}
