﻿using System;
using SIS.Framework;
using SIS.Framework.Routers;
using SIS.HTTP.Enums;
using SIS.WebServer;
using SIS.WebServer.Routing;

namespace SIS.Demo
{
    class Launcher
    {
        static void Main(string[] args)
        {
            /*ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController().Index(request);

            Server server = new Server(8000, serverRoutingTable);

            server.Run();*/
            
            WebHost.Start(new StartUp());
        }
    }
}
