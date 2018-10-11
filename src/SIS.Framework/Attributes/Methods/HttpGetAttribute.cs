using SIS.Framework.Attributes.Methods.Base;

namespace SIS.Framework.Attributes.Methods
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
        {
            if (requestMethod.ToLower() == "post")
            {
                return true;
            }

            return false;
        }
    }
}