using SpyRadioStationApi.Models.db;
using SpyRadioStationApi.Models.enums;

namespace SpyRadioStationApi.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task CreateAsync(Notification notification);
        Task UpdateAsync(Status status, params int[] ids);
        Task<IList<Notification>> GetAllAsync();
        Task<IList<Notification>> GetByStatusAsync(Status status);
        Task DeleteByStatusAsync(Status status);
    }
}
