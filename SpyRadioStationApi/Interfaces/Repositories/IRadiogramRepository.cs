using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Interfaces.Repositories
{
    public interface IRadiogramRepository
    {
        Task Create(Radiogram radiogram);
        Task<IReadOnlyCollection<Radiogram>> GetAll();
        Task<Radiogram> GetById(Guid Id);

        Task<Radiogram> GetLastByDate(DateTime date);
    }
}
