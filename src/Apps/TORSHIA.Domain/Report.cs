using System;

namespace TORSHIA.Domain
{
    public class Report
    {
        public string Id { get; set; }
        
        public DateTime ReportedOn { get; set; }

        public string StatusId { get; set; }

        public ReportStatus Status { get; set; }

        public string TaskId { get; set; }

        public Task Task { get; set; }

        public string ReporterId { get; set; }

        public User Reporter { get; set; }
    }
}