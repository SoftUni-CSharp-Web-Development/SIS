using System;

namespace SIS.Framework.Attributes
{
    public abstract class HttpMethodAttribute : Attribute
    {
        protected HttpMethodAttribute()
        {
        }

        public abstract bool IsValid(string requestMethod);
    }
}