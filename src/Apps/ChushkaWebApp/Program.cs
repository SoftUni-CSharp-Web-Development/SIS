using System;
using SIS.MvcFramework;

namespace ChushkaWebApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WebHost.Start(new Startup());
        }
    }
}
