using System;
using SIS.Framework.Security.Base;

namespace SIS.Framework.Security
{
    public class IdentityUser : IdentityUserT<string>
    {
        public IdentityUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
