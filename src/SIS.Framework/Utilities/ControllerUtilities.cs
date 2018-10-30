namespace SIS.Framework.Utilities
{
    public class ControllerUtilities
    {
        public static string GetControllerName(object controller)
            => controller.GetType()
                .Name.Replace(MvcContext.Get.ControllersSuffix, string.Empty);
    }
}
