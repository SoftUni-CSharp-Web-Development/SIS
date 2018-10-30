using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TORSHIA.App.Models.Binding;
using TORSHIA.Data;
using TORSHIA.Domain;
using TORSHIA.Services.Contracts;

namespace TORSHIA.Services
{
    public class TasksService : ITasksService
    {
        private readonly TorshiaDbContext context;

        private readonly IReportsService reportsService;

        public TasksService(TorshiaDbContext context, IReportsService reportsService)
        {
            this.context = context;
            this.reportsService = reportsService;
        }

        public void CreateTask(CreateTaskBindingModel createTaskBindingModel)
        {
            Task task = new Task
            {
                Title = createTaskBindingModel.Title,
                Description = createTaskBindingModel.Description,
                DueDate = createTaskBindingModel.DueDate,
                ParticipantsString = createTaskBindingModel.Participants,
                IsReported = false,
            };

            if (createTaskBindingModel.AffectedSectors != null)
            {
                foreach (var affectedSector in createTaskBindingModel.AffectedSectors)
                {
                    task.AffectedSectors.Add(new TaskSector
                    {
                        Sector = this.context.Sectors.SingleOrDefault(s => s.Name == affectedSector),
                        Task = task
                    });
                }
            }

            this.context.Tasks.Add(task);
            this.context.SaveChanges();
        }

        public ICollection<Task> GetAllUnreportedTasks()
            => this.context
                .Tasks
                .Include(t => t.AffectedSectors)
                .Where(t => t.IsReported == false)
                .ToList();

        public Task GetTaskById(string taskId)
            => this.context.Tasks
                .Include(t => t.AffectedSectors)
                .SingleOrDefault(t => t.Id == taskId);

        public void ReportTask(string taskId, string reporterUsername)
        {
            Task task = this.GetTaskById(taskId);

            if (task == null)
            {
                return;
            }

            task.IsReported = true;

            this.reportsService.CreateReport(task.Id, reporterUsername);
            context.SaveChanges();
        }
    }
}
