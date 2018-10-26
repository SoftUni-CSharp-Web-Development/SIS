namespace SIS.MvcFramework.Services
{
    public interface IUserCookieService
    {
        string GetUserCookie(MvcUserInfo user);

        MvcUserInfo GetUserData(string cookieContent);
    }
}
