using System.Collections.Generic;

namespace MishMashWebApp.Models
{
    public class User
    {
        public User()
        {
            this.Channels = new HashSet<UserInChannel>();
        }

        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public virtual ICollection<UserInChannel> Channels { get; set; }

        public Role Role { get; set; }
    }
}
