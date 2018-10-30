namespace SIS.Framework.Attributes.Method
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        private const string HttpGetAttributeRequestMethod = "GET";

        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, HttpGetAttributeRequestMethod);
        }
    }
}