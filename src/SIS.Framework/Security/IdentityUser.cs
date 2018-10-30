using System;
using System.Collections.Generic;

namespace SIS.Framework.Security
{
    public class IdentityUser : IdentityUser<string>
    {
        public IdentityUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }

    public class IdentityUser<TKey> : IIdentity where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }

        public virtual string Username { get; set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual bool IsValid { get; set; }

        public virtual IEnumerable<string> Roles { get; set; }
    }
}