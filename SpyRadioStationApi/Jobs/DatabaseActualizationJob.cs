
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Jobs
{
    public class DatabaseActualizationJob : BaseJob
    {
        private readonly IDatabaseBootstrap _bootstrap;
        public DatabaseActualizationJob(IDatabaseBootstrap bootstrap, INotificationRepository notification) : base(notification)
        {
            _bootstrap = bootstrap;
        }
        protected override async Task ExecuteAsync()
        {
            var isUpdated = await _bootstrap.Setup();

            if (isUpdated)
                await _notificationRepository.CreateAsync(new Notification { Message = "Database actualization job successfully completed!",
                    NotificationType = Models.enums.NotificationType.Info });
        }
    }
}
