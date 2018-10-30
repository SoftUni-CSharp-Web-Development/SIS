using System;

namespace SIS.Framework.Attributes.Method
{
    public abstract class HttpMethodAttribute  : Attribute
    {
        protected virtual bool IsValidMethod(string requestMethod, string attributeName)
        {
            return requestMethod.ToUpper() == attributeName;
        }

        public abstract bool IsValid(string requestMethod);
    }
}