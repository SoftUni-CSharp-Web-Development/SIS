namespace SIS.Framework.Attributes
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        private const string GetString = "GET";


        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == GetString;
        }
    }
}