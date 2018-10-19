namespace SIS.MvcFramework.ViewEngine
{
    public interface IView<T>
    {
        string GetHtml(T model, string user);
    }
}
