using SIS.Framework.Routers.Contracts;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Api;

namespace SIS.Framework
{
    public class HttpRequestHandlingContext : IHttpRequestHandler
    {
        private readonly IMvcRouter mvcRouter;

        private readonly IResourceRouter resourceRouter;

        private readonly ICustomRouter customRouter;

        public HttpRequestHandlingContext(IMvcRouter mvcRouter, 
            IResourceRouter resourceRouter, 
            ICustomRouter customRouter)
        {
            this.mvcRouter = mvcRouter;
            this.resourceRouter = resourceRouter;
            this.customRouter = customRouter;
        }

        public IHttpResponse Handle(IHttpRequest httpRequest)
        {
            IHttpResponse response = null;

            if (this.resourceRouter.IsResourceRequest(httpRequest.Path))
            {
                response = this.resourceRouter.Handle(httpRequest);
            }
            else if (this.customRouter.ContainsMapping(httpRequest))
            {
                response = this.customRouter.Handle(httpRequest);
            }
            else
            {
                response = this.mvcRouter.Handle(httpRequest);
            }

            return response ?? new HttpResponse(HttpResponseStatusCode.NotFound);
        }
    }
}
