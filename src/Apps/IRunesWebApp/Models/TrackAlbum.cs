namespace IRunesWebApp.Models
{
    public class TrackAlbum
    {
        public string AlbumId { get; set; }

        public virtual Album Album { get; set; }

        public string TrackId { get; set; } 

        public virtual Track Track { get; set; }
    }
}
