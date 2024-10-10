using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.db;
using SpyRadioStationApi.Models.enums;

namespace SpyRadioStationApi.Jobs
{
    public class RemoveCodeMessageJob : BaseJob
    {
        private readonly IRadiogramRepository _radiogramRepository;
        public RemoveCodeMessageJob(IRadiogramRepository radiogramRepository,
            INotificationRepository notificationRepository):base(notificationRepository) 
        {
            _radiogramRepository = radiogramRepository;
        }
        protected override async Task ExecuteAsync()
        {
            var date = DateTime.Now.AddDays(-2);
            await _radiogramRepository.DeleteUntilAsync(date);
            await _notificationRepository.CreateAsync(new Notification { Message = $"Data older than {date.ToString("dd-MM-yyyy")} has been removed", NotificationType = NotificationType.Info });
        }
    }
}
