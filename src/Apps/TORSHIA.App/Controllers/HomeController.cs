using System.Collections.Generic;
using System.Linq;
using SIS.Framework.ActionResults;
using TORSHIA.App.Models.View;
using TORSHIA.Services.Contracts;

namespace TORSHIA.App.Controllers
{
    public class HomeController : BaseController
    {
        public readonly ITasksService tasksService;

        public HomeController(ITasksService tasksService)
        {
            this.tasksService = tasksService;
        }

        public IActionResult Index()
        {
            if (this.Identity != null)
            {
                this.Model["Username"] = this.Identity.Username;

                List<IndexTaskViewModel> taskViewModels =
                    this.tasksService.GetAllUnreportedTasks()
                        .Select(t => new IndexTaskViewModel
                        {
                            Id = t.Id,
                            Title = t.Title,
                            Level = t.AffectedSectors.Count
                        })
                        .ToList();

                List<IndexTasksRowViewModel> taskRowViewModels = new List<IndexTasksRowViewModel>();

                for (int i = 0; i < taskViewModels.Count; i++)
                {
                    if (i % 5 == 0)
                    {
                        taskRowViewModels.Add(new IndexTasksRowViewModel());
                    }

                    taskRowViewModels[taskRowViewModels.Count - 1].Tasks.Add(taskViewModels[i]);
                }

                this.Model["TaskRows"] = taskRowViewModels;

                if (this.Identity.Roles.Contains("Admin"))
                {
                    return this.View("Index-Admin");
                }

                if (this.Identity.Roles.Contains("User"))
                {
                    return this.View("Index-User");
                }
            }

            return this.View();
        }
    }
}
