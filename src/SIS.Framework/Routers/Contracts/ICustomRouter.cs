using SIS.HTTP.Requests;
using SIS.WebServer.Api;

namespace SIS.Framework.Routers.Contracts
{
    public interface ICustomRouter : IHttpRequestHandler
    {
        bool ContainsMapping(IHttpRequest httpRequest);
    }
}
