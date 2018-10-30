namespace TORSHIA.Domain
{
    public class TaskSector
    {
        public string TaskId { get; set; }

        public Task Task { get; set; }

        public string SectorId { get; set; }

        public Sector Sector { get; set; }
    }
}
