using System;
using System.Linq;
using SIS.Framework.Security;

namespace SIS.Framework.Attributes.Action
{
    public class AuthorizeAttribute : Attribute
    {
        private readonly string[] roles;

        private string[] FormatRoles(string[] inputRoles)
        {
            return inputRoles.Length > 0 
                ? inputRoles.Select(r => r.ToLower()).ToArray()
                : inputRoles;
        }

        public AuthorizeAttribute()
        {
            this.roles = new string[0];
        }
        
        public AuthorizeAttribute(params string[] roles)
        {
            this.roles = this.FormatRoles(roles);
        }

        private bool IsIdentityPresent(IIdentity identity)
            => identity != null;

        private bool IsIdentityInRole(IIdentity identity)
            => this.IsIdentityPresent(identity)
               && identity.Roles.Any(r => this.roles.Contains(r.ToLower()));

        public bool IsAuthorized(IIdentity user)
            => this.roles.Length > 0
                ? this.IsIdentityInRole(user)
                : this.IsIdentityPresent(user);
    }
}