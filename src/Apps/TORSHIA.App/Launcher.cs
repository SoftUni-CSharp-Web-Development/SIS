using SIS.Framework;

namespace TORSHIA.App
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            WebHost.Start(new StartUp());
        }
    }
}
