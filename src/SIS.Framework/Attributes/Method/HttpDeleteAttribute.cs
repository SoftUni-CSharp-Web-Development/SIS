namespace SIS.Framework.Attributes.Method
{
    public class HttpDeleteAttribute : HttpMethodAttribute
    {
        private const string HttpDeleteAttributeRequestMethod = "DELETE";

        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, HttpDeleteAttributeRequestMethod);
        }
    }
}
