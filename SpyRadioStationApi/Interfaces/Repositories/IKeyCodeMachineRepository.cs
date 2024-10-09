using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Interfaces.Repositories
{
    public interface IKeyCodeMachineRepository
    {
        Task<Radiogram> GetByDay(int day);
    }
}
