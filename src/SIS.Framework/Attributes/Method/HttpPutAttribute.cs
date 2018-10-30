namespace SIS.Framework.Attributes.Method
{
    public class HttpPutAttribute : HttpMethodAttribute
    {
        private const string HttpPutAttributeRequestMethod = "PUT";

        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, HttpPutAttributeRequestMethod);
        }
    }
}
