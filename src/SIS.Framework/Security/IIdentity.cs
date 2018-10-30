using System.Collections.Generic;

namespace SIS.Framework.Security
{
    public interface IIdentity
    {
        string Username { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        bool IsValid { get; set; }

        IEnumerable<string> Roles { get; set; }
    }
}
