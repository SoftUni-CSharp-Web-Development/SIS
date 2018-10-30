using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SIS.Framework.Security;

namespace TORSHIA.Domain
{
    public class User : IdentityUser
    {
        public string RoleId { get; set; }

        public UserRole Role { get; set; }

        [NotMapped]
        public override IEnumerable<string> Roles => new[] {this.Role.Name};
    }
}
