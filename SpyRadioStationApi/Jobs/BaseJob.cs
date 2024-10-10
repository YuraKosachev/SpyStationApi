using Coravel.Invocable;
using SpyRadioStationApi.Interfaces.Repositories;

namespace SpyRadioStationApi.Jobs
{
    public abstract class BaseJob : IInvocable
    {
        protected readonly INotificationRepository _notificationRepository;
        public BaseJob(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public async Task Invoke()
        {
            try
            {
                await ExecuteAsync();
            }
            catch (Exception ex)
            {
                await _notificationRepository.CreateAsync(new Models.db.Notification { Message = ex.Message, NotificationType = Models.enums.NotificationType.Error });
            }
        }

        protected abstract Task ExecuteAsync();
    }
}
