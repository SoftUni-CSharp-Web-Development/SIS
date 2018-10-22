namespace SIS.Framework
{
    public class MvcContext
    {
        private static MvcContext Instance;

        private MvcContext() { }

        public static MvcContext Get => Instance ?? (Instance = new MvcContext());

        public string AssemblyName { get; set; }

        public string ControllersFolders { get; set; }

        public string ControllerSuffix { get; set; }

        public string ViewsFolder { get; set; }

        public string ModelFolder { get; set; }



    }
}