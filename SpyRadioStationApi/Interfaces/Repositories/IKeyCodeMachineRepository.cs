using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Interfaces.Repositories
{
    public interface IKeyCodeMachineRepository
    {
        Task<Key> GetByDayAsync(int day);
    }
}
