using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SIS.Framework.ActionResults;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Method;
using TORSHIA.App.Models.Binding;
using TORSHIA.App.Models.View;
using TORSHIA.Data;
using TORSHIA.Domain;
using TORSHIA.Services.Contracts;

namespace TORSHIA.App.Controllers
{
    public class TasksController : BaseController
    {
        public readonly ITasksService tasksService;

        public readonly ISectorsService sectorsService;

        public TasksController(ITasksService tasksService, ISectorsService sectorsService)
        {
            this.tasksService = tasksService;
            this.sectorsService = sectorsService;
        }

        [HttpGet]
        [Authorize("Admin")]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize("Admin")]
        public IActionResult Create(CreateTaskBindingModel bindingModel)
        {
            if (this.ModelState.IsValid != true)
            {
                return this.View();
            }

            this.tasksService.CreateTask(bindingModel);
            return this.RedirectToAction("/Home/Index");
        }

        [HttpGet]
        [Authorize("Admin", "User")]
        public IActionResult Details()
        {
            string taskId = this.Request.QueryData["id"].ToString();

            Task task = this.tasksService.GetTaskById(taskId);

            if (task == null)
            {
                return this.RedirectToAction("/Home/Index");
            }

            List<string> affectedSectors = task.AffectedSectors
                .Select(ts => this.sectorsService.GetSectorById(ts.SectorId).Name)
                .ToList();

            DetailsTaskViewModel taskViewModel = new DetailsTaskViewModel
            {
                Title = task.Title,
                Description = task.Description,
                Participants = task.ParticipantsString,
                Level = task.AffectedSectors.Count,
                DueDate = task.DueDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                AffectedSectors = string.Join(", ", affectedSectors)
            };

            this.Model["Task"] = taskViewModel;
            return this.View();
        }

        [HttpGet]
        [Authorize("Admin", "User")]
        public IActionResult Report()
        {
            string taskId = this.Request.QueryData["id"].ToString();

            this.tasksService.ReportTask(taskId, this.Identity.Username);
            return this.RedirectToAction("/Home/Index");
        }
    }
}