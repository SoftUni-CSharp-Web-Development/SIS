using IRunesWebApp.Models;

using Microsoft.EntityFrameworkCore;

namespace IRunesWebApp.Data
{
    // ReSharper disable once InconsistentNaming
    public class IRunesContext : DbContext
    {
        public DbSet<User> Users { get; set; }  

        public DbSet<Album> Albums { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<TrackAlbum> TracksAlbums { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=.; Database=IRunesApp; Integrated Security=true")
                .UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrackAlbum>()
                .HasKey(ta => new { ta.AlbumId, ta.TrackId });
        }
    }
}
