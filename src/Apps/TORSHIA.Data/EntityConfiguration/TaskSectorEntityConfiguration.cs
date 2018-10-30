using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TORSHIA.Domain;

namespace TORSHIA.Data.EntityConfiguration
{
    public class TaskSectorEntityConfiguration : IEntityTypeConfiguration<TaskSector>
    {
        public void Configure(EntityTypeBuilder<TaskSector> builder)
        {
            builder
                .HasKey(ts => new {ts.TaskId, ts.SectorId});

            builder
                .HasOne(ts => ts.Task)
                .WithMany(t => t.AffectedSectors)
                .HasForeignKey(ts => ts.TaskId);

            builder
                .HasOne(ts => ts.Sector)
                .WithMany()
                .HasForeignKey(ts => ts.SectorId);
        }
    }
}
