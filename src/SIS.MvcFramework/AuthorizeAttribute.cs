using System;

namespace SIS.MvcFramework
{
    public class AuthorizeAttribute : Attribute
    {
        public string RoleName { get; }

        public AuthorizeAttribute(string roleName = null)
        {
            this.RoleName = roleName;
        }
    }
}
