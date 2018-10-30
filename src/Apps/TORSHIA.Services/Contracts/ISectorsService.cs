using TORSHIA.Domain;

namespace TORSHIA.Services.Contracts
{
    public interface ISectorsService
    {
        Sector GetSectorById(string sectorId);
    }
}
