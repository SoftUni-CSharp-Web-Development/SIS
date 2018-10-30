using System.Collections.Generic;
using TORSHIA.App.Models.Binding;
using TORSHIA.Domain;

namespace TORSHIA.Services.Contracts
{
    public interface ITasksService
    {
        void CreateTask(CreateTaskBindingModel createTaskBindingModel);

        ICollection<Task> GetAllUnreportedTasks();

        Task GetTaskById(string taskId);

        void ReportTask(string taskId, string reporterUsername);
    }
}
