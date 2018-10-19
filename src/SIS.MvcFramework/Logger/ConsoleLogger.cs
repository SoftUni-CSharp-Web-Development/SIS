using System;

namespace SIS.MvcFramework.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] {message}");
        }
    }
}