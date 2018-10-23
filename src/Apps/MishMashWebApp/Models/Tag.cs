using System.Collections.Generic;

namespace MishMashWebApp.Models
{
    public class Tag
    {
        public Tag()
        {
            this.Channels = new HashSet<ChannelTag>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ChannelTag> Channels { get; set; }
    }
}