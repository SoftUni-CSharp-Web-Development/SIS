namespace SIS.Framework.Attributes.Method
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        private const string HttpPostAttributeRequestMethod = "POST";

        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, HttpPostAttributeRequestMethod);
        }
    }
}