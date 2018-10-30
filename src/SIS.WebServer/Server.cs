using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SIS.WebServer.Api;

namespace SIS.WebServer
{
    public class Server
    {
        private const string LocalhostIpAddress = "127.0.0.1";

        private readonly int port;

        private readonly TcpListener listener;

        private readonly IHttpRequestHandler httpRequestHandler;

        private bool isRunning;

        private Server(int port)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalhostIpAddress), this.port);
        }

        public Server(int port, IHttpRequestHandler httpRequestHandler) : this(port)
        {
            this.httpRequestHandler = httpRequestHandler;
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
            ConnectionHandler connectionHandler = new ConnectionHandler(client, this.httpRequestHandler);
            await connectionHandler.ProcessRequestAsync();
        }
    }
}
