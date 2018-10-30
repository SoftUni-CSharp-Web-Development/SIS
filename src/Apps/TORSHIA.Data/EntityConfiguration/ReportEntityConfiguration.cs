using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TORSHIA.Domain;

namespace TORSHIA.Data.EntityConfiguration
{
    public class ReportEntityConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder
                .HasKey(r => r.Id);

            builder
                .HasOne(r => r.Reporter)
                .WithMany()
                .HasForeignKey(r => r.ReporterId);

            builder
                .HasOne(r => r.Status)
                .WithMany()
                .HasForeignKey(r => r.StatusId);
            
            builder
                .HasOne(r => r.Task)
                .WithMany()
                .HasForeignKey(r => r.TaskId);
        }
    }
}
