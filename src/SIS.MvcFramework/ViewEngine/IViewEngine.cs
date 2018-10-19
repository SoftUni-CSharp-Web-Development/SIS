namespace SIS.MvcFramework.ViewEngine
{
    public interface IViewEngine
    {
        string GetHtml<T>(string viewName, string viewCode, T model, string user = null);
    }
}
