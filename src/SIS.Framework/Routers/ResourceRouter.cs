using System.IO;
using System.Text;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Api;
using SIS.WebServer.Results;

namespace SIS.Framework.Routers
{
    public class ResourceRouter : IHttpHandler
    {
        public IHttpResponse Handle(IHttpRequest request)
        {
            //TODO: Configure to abstract approach.
            if (!File.Exists(path: @"D:/SIS-Cake/SIS/src/IRunes/Resources" + request.Path))
                return new HttpResponse(HttpResponseStatusCode.NotFound);

            var content = File.ReadAllText(@"D:/SIS-Cake/SIS/src/IRunes/Resources" + request.Path);
            var bytes = Encoding.UTF8.GetBytes(content);
            return new InlineResourceResult(bytes, HttpResponseStatusCode.Ok);
        }
    }
}