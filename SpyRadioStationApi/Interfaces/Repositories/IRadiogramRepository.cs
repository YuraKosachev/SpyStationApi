using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Interfaces.Repositories
{
    public interface IRadiogramRepository
    {
        Task CreateAsync(Radiogram radiogram);
        Task CreateRangeAsync(IList<Radiogram> radiograms);
        Task<IList<Radiogram>> GetAllAsync();
        Task<Radiogram> GetByIdAsync(int Id);
        Task<bool> ValidationAsync(string encode, string decode);
        Task<Radiogram> GetLastByDateAsync();

        Task DeleteUntilAsync(DateTime date);
    }
}
