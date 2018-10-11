using SIS.Framework.Attributes.Methods.Base;

namespace SIS.Framework.Attributes.Methods
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
        {
            if (requestMethod.ToLower() == "get")
            {
                return true;
            }

            return false;
        }
    }
}