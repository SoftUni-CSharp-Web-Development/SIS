namespace SIS.MvcFramework.ViewEngine
{
    public interface IView<T>
    {
        string GetHtml(T model, MvcUserInfo user);
    }
}
