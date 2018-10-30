using SIS.Framework.Api;
using SIS.Framework.Services;
using TORSHIA.Data;
using TORSHIA.Domain;
using TORSHIA.Services;
using TORSHIA.Services.Contracts;

namespace TORSHIA.App
{
    public class StartUp : MvcApplication
    {
        private void SeedDatabase()
        {
            using (var context = new TorshiaDbContext())
            {
                context.Database.EnsureCreated();

                context.UserRoles.Add(new UserRole { Name = "User" });
                context.UserRoles.Add(new UserRole { Name = "Admin" });

                context.Sectors.Add(new Sector { Name = "Customers" });
                context.Sectors.Add(new Sector { Name = "Marketing" });
                context.Sectors.Add(new Sector { Name = "Finances" });
                context.Sectors.Add(new Sector { Name = "Internal" });
                context.Sectors.Add(new Sector { Name = "Management" });

                context.ReportStatuses.Add(new ReportStatus { Status = "Completed" });
                context.ReportStatuses.Add(new ReportStatus { Status = "Archived" });

                context.SaveChanges();
            }
        }

        public override void Configure()
        {
            //using (var context = new TorshiaDbContext())
            //{
            //    context.Database.Migrate();
            //}

            //this.SeedDatabase();
        }

        public override void ConfigureServices(IDependencyContainer dependencyContainer)
        {
            dependencyContainer.RegisterDependency<TorshiaDbContext, TorshiaDbContext>();

            dependencyContainer.RegisterDependency<IUsersService, UsersService>();
            dependencyContainer.RegisterDependency<ITasksService, TasksService>();
            dependencyContainer.RegisterDependency<IReportsService, ReportsService>();
            dependencyContainer.RegisterDependency<ISectorsService, SectorsService>();

            base.ConfigureServices(dependencyContainer);
        }
    }
}
