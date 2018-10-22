namespace SIS.Framework.Attributes
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        private const string PostString = "POST";

        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == PostString;
        }
    }
}