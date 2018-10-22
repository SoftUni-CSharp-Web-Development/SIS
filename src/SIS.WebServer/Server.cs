using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SIS.WebServer.Api;
using SIS.WebServer.Routing;

namespace SIS.WebServer
{
    public class Server
    {
        private const string LocalhostIpAddress = "127.0.0.1";

        private readonly int port;
        
          private readonly IHttpHandler handler ;
        private readonly IHttpHandler resourceHandler;
        private readonly TcpListener listener;

        private readonly ServerRoutingTable serverRoutingTable;

        private bool isRunning;
        /*
                public Server(int port, ServerRoutingTable serverRoutingTable)
                {
                    this.port = port;
                    this.listener = new TcpListener(IPAddress.Parse(LocalhostIpAddress), port);

                    this.serverRoutingTable = serverRoutingTable;
                }
                */

        //, IHttpHandler resourceHandler
        public Server(int port, IHttpHandler handler)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalhostIpAddress), port);

            this.handler = handler;
            //this.resourceHandler = resourceHandler;
        }


        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine($"Server started at http://{LocalhostIpAddress}:{this.port}");
            while (isRunning)
            {
                Console.WriteLine("Waiting for client...");

                var client = listener.AcceptSocketAsync().GetAwaiter().GetResult();

                Task.Run(() => Listen(client));
            }
        }

        public async void Listen(Socket client)
        {
            ConnectionHandler connectionHandler = null;

            connectionHandler = this.serverRoutingTable != null ? 
                new ConnectionHandler(client, this.serverRoutingTable) : 
                new ConnectionHandler(client, this.handler,this.resourceHandler);

            await connectionHandler.ProcessRequestAsync();
        }
    }
}
