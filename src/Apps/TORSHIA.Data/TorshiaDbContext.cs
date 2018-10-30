using Microsoft.EntityFrameworkCore;
using TORSHIA.Data.EntityConfiguration;
using TORSHIA.Domain;

namespace TORSHIA.Data
{
    public class TorshiaDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Sector> Sectors { get; set; }

        public DbSet<TaskSector> TasksSectors { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<ReportStatus> ReportStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=.\\SQLEXPRESS;Database=TorshiaDB;Trusted_Connection=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ReportStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SectorEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ReportEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskSectorEntityConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
