using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SIS.Framework.ActionResults;
using SIS.Framework.Attributes.Action;
using TORSHIA.App.Models.View;
using TORSHIA.Domain;
using TORSHIA.Services.Contracts;

namespace TORSHIA.App.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly IReportsService reportsService;

        private readonly ISectorsService sectorsService;

        public ReportsController(IReportsService reportsService, ISectorsService sectorsService)
        {
            this.reportsService = reportsService;
            this.sectorsService = sectorsService;
        }

        [Authorize("Admin")]
        public IActionResult All()
        {
            List<AllReportViewModel> reportViewModels =
                this.reportsService.GetAllReports()
                .Select(r => new AllReportViewModel
                {
                    Id = r.Id,
                    Title = r.Task.Title,
                    Level = r.Task.AffectedSectors.Count,
                    Status = r.Status.Status
                })
                .ToList();

            for (int i = 0; i < reportViewModels.Count; i++)
            {
                reportViewModels[i].Index = i + 1;
            }

            this.Model["Reports"] = reportViewModels;
            return this.View();
        }

        public IActionResult Details()
        {
            string reportId = this.Request.QueryData["id"].ToString();

            Report report = this.reportsService.GetReportById(reportId);

            if (report == null)
            {
                return RedirectToAction("/Reports/All");
            }

            List<string> affectedSectors = report.Task.AffectedSectors
                .Select(ts => this.sectorsService.GetSectorById(ts.SectorId).Name)
                .ToList();

            DetailsReportViewModel reportViewModel = new DetailsReportViewModel
            {
                Id = report.Id,
                Description = report.Task.Description,
                DueDate = report.Task.DueDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ReportDate = report.ReportedOn.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                AffectedSectors = string.Join(", ", affectedSectors),
                Participants = report.Task.ParticipantsString,
                Reporter = report.Reporter.Username,
                Status = report.Status.Status,
                Task = report.Task.Title,
                Level = report.Task.AffectedSectors.Count
            };

            this.Model["Report"] = reportViewModel;

            return this.View();
        }
    }
}
