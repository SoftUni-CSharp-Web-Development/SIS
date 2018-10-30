using System.Collections.Generic;
using TORSHIA.Domain;

namespace TORSHIA.Services.Contracts
{
    public interface IReportsService
    {
        void CreateReport(string taskId, string reporterUsername);

        ICollection<Report> GetAllReports();

        Report GetReportById(string reportId);
    }
}
