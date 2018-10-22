    using System;
    using System.IO;
    using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
    using SIS.WebServer.Api;

namespace SIS.WebServer
{
    using HTTP.Common;
    using HTTP.Exceptions;
    using HTTP.Requests;
    using HTTP.Responses;
    using HTTP.Sessions;
    using Results;
    using Routing;

    public class ConnectionHandler
    {
        private readonly Socket client;

        private readonly ServerRoutingTable serverRoutingTable;
        private readonly IHttpHandler handler;
        private readonly IHttpHandler resourceHandler;

        public ConnectionHandler(
            Socket client,
            ServerRoutingTable serverRoutingTable)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(serverRoutingTable, nameof(serverRoutingTable));

            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
        }


        public ConnectionHandler(
            Socket client,
            IHttpHandler handler,
            IHttpHandler resourceHandler)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(handler, nameof(handler));

            this.client = client;
            this.handler = handler;
            this.resourceHandler = resourceHandler;
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesRead = await this.client.ReceiveAsync(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0)
                {
                    break;
                }

                var bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);
                result.Append(bytesAsString);

                if (numberOfBytesRead < 1023)
                {
                    break;
                }
            }

            if (result.Length == 0)
            {
                return null;
            }

            return new HttpRequest(result.ToString());
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            if (this.serverRoutingTable != null)
            {
                if (!this.serverRoutingTable.Routes.ContainsKey(httpRequest.RequestMethod)
                    || !this.serverRoutingTable.Routes[httpRequest.RequestMethod].ContainsKey(httpRequest.Path.ToLower()))
                {
                    return this.resourceHandler.Handle(httpRequest);
                }

                return this.serverRoutingTable.Routes[httpRequest.RequestMethod][httpRequest.Path.ToLower()].Invoke(httpRequest);
            }
            else
            {
                return this.handler.Handle(request: httpRequest);
            }
           
        }

        /*private IHttpResponse ReturnIfResponse(string path)
        {
            if (!File.Exists(path: @"D:/SIS-Cake/SIS/src/IRunes/Resources" + path))
                return new HttpResponse(HttpResponseStatusCode.NotFound);

            var content = File.ReadAllText(@"D:/SIS-Cake/SIS/src/IRunes/Resources" + path);
            var bytes = Encoding.UTF8.GetBytes(content);
            return new InlineResourceResult(bytes, HttpResponseStatusCode.Ok);

        }*/

        private async Task PrepareResponse(IHttpResponse httpResponse)
        {
            byte[] byteSegments = httpResponse.GetBytes();

            await this.client.SendAsync(byteSegments, SocketFlags.None);
        }

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            string sessionId = null;

            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest.Cookies.GetCookie(HttpSessionStorage.SessionCookieKey);
                sessionId = cookie.Value;
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private void SetResponseSession(IHttpResponse httpResponse, string sessionId)
        {
            if (sessionId != null)
            {
                httpResponse
                    .AddCookie(new HttpCookie(HttpSessionStorage.SessionCookieKey
                        , $"{sessionId}; HttpOnly"));
            }
        }

        public async Task ProcessRequestAsync()
        {
            try
            {
                var httpRequest = await this.ReadRequest();

                if (httpRequest != null)
                {
                    string sessionId = this.SetRequestSession(httpRequest);

                    var httpResponse = this.HandleRequest(httpRequest);

                    this.SetResponseSession(httpResponse, sessionId);

                    await this.PrepareResponse(httpResponse);
                }
            }
            catch (BadRequestException e)
            {
                await this.PrepareResponse(new TextResult(e.Message, HttpResponseStatusCode.BadRequest));
            }

            catch (Exception e)
            {
                await this.PrepareResponse(new TextResult(e.Message, HttpResponseStatusCode.InternalServerError));
            }

            this.client.Shutdown(SocketShutdown.Both);
        }
    }
}