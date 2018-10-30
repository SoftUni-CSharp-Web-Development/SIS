using System.Linq;
using TORSHIA.Data;
using TORSHIA.Domain;
using TORSHIA.Services.Contracts;

namespace TORSHIA.Services
{
    public class SectorsService : ISectorsService
    {
        private readonly TorshiaDbContext context;

        public SectorsService(TorshiaDbContext context)
        {
            this.context = context;
        }

        public Sector GetSectorById(string sectorId)
            => this.context
                .Sectors
                .SingleOrDefault(s => s.Id == sectorId);
    }
}
