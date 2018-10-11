using System.Collections.Generic;

namespace IRunesWebApp.Models
{
    public class Track : BaseEntity<string>
    {
        public Track()
        {
            this.Albums = new HashSet<TrackAlbum>();
        }

        public string Name { get; set; }

        public string Link { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<TrackAlbum> Albums { get; set; }
    }
}
