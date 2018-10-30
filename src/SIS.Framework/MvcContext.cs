using System.Reflection;

namespace SIS.Framework
{
    public class MvcContext
    {
        private static MvcContext instance;

        private MvcContext() { }

        public static MvcContext Get => instance ?? (instance = new MvcContext());

        public string AssemblyName { get; set; } = Assembly.GetEntryAssembly().GetName().Name;

        public string ControllerFolder { get; set; } = "Controllers";

        public string ControllersSuffix { get; set; } = "Controller";

        public string ViewsFolder { get; set; } = "Views";

        public string ModelsFolder { get; set; } = "Models";
    }
}
