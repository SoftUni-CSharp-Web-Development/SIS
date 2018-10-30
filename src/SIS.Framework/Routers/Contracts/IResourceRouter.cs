using SIS.WebServer.Api;

namespace SIS.Framework.Routers.Contracts
{
    public interface IResourceRouter : IHttpRequestHandler
    {
        bool IsResourceRequest(string httpRequestPath);
    }
}
