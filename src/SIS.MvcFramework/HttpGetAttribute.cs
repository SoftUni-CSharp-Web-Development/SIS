using SIS.HTTP.Enums;

namespace SIS.MvcFramework
{
    public class HttpGetAttribute : HttpAttribute
    {
        public HttpGetAttribute(string path)
            : base(path)
        {
        }

        public override HttpRequestMethod Method => HttpRequestMethod.Get;
    }
}
