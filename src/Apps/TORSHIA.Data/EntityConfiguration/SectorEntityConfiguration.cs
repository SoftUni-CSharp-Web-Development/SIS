using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TORSHIA.Domain;

namespace TORSHIA.Data.EntityConfiguration
{
    public class SectorEntityConfiguration : IEntityTypeConfiguration<Sector>
    {
        public void Configure(EntityTypeBuilder<Sector> builder)
        {
            builder
                .HasKey(s => s.Id);
        }
    }
}
