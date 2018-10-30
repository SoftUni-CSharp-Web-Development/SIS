using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TORSHIA.Data;
using TORSHIA.Domain;
using TORSHIA.Services.Contracts;

namespace TORSHIA.Services
{
    public class ReportsService : IReportsService
    {
        private readonly TorshiaDbContext context;

        public ReportsService(TorshiaDbContext context)
        {
            this.context = context;
        }

        public void CreateReport(string taskId, string reporterUsername)
        {
            Report report = new Report
            {
                ReportedOn = DateTime.Now,
                Status = (new Random().Next(1, 5) == 4
                    ? this.context.ReportStatuses.SingleOrDefault(rs => rs.Status == "Archived")
                    : this.context.ReportStatuses.SingleOrDefault(rs => rs.Status == "Completed")),
                Task = this.context.Tasks.SingleOrDefault(t => t.Id == taskId),
                Reporter = this.context.Users.SingleOrDefault(u => u.Username == reporterUsername)
            };

            this.context.Reports.Add(report);
            this.context.SaveChanges();
        }

        public ICollection<Report> GetAllReports()
            => this.context
                .Reports
                .Include(r => r.Status)
                .Include(r => r.Task)
                .Include(r => r.Task.AffectedSectors)
                .Include(r => r.Reporter)
                .ToList();

        public Report GetReportById(string reportId)
            => this.context
                .Reports
                .Include(r => r.Status)
                .Include(r => r.Task)
                .Include(r => r.Task.AffectedSectors)
                .Include(r => r.Reporter)
                .SingleOrDefault(r => r.Id == reportId);
    }
}
