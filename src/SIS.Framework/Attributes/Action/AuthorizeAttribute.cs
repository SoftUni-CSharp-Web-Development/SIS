using System;
using System.Linq;
using SIS.Framework.Security;

namespace SIS.Framework.Attributes.Action
{
    public class AuthorizeAttribute : Attribute
    {
        private readonly string role;

        public AuthorizeAttribute()
        {
            
        }

        public AuthorizeAttribute(string role)
        {
            this.role = role;
        }

        private bool IsIdentityPresent(IIdentity identity)
        {
            return identity != null;
        }

        private bool IsIdentityInRole(IIdentity identity)
        {
            if (this.role != null)
            {
                return !this.IsIdentityPresent(identity) && identity.Roles.Contains(this.role);
            }

            return false;

        }

        public bool IsAuthorized(IIdentity user)
        {
            if (this.role == null)
            {
                return true;
            }

            return this.IsIdentityInRole(user);

        }


    }
}