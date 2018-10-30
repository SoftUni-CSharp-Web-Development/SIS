using System;
using System.Collections.Generic;
using SIS.Framework.Routers.Contracts;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace SIS.Framework.Routers
{
    public class CustomRouter : ICustomRouter
    {
        public CustomRouter()
        {
            this.Routes = new Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>>
            {
                [HttpRequestMethod.Get] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Post] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Put] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Delete] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>()
            };
        }

        private Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>> Routes { get; }

        public bool ContainsMapping(IHttpRequest httpRequest)
            => this.Routes.ContainsKey(httpRequest.RequestMethod)
                   && this.Routes[httpRequest.RequestMethod].ContainsKey(httpRequest.Path.ToLower());

        public IHttpResponse Handle(IHttpRequest httpRequest)
        {
            if (this.ContainsMapping(httpRequest))
            {
                return this.Routes[httpRequest.RequestMethod][httpRequest.Path].Invoke(httpRequest);
            }

            return null;
        }
    }
}
