using SIS.HTTP.Enums;

namespace SIS.MvcFramework
{
    public class HttpGetAttribute : HttpAttribute
    {
        public HttpGetAttribute(string path = null)
            : base(path)
        {
        }

        public override HttpRequestMethod Method => HttpRequestMethod.Get;
    }
}
