using Coravel.Invocable;
using SpyRadioStationApi.Interfaces.Repositories;

namespace SpyRadioStationApi.Jobs
{
    public abstract class BaseJob : IInvocable
    {
        protected readonly INotificationRepository _notificationRepository;
        protected readonly ILogger<BaseJob> _logger;
        public BaseJob(INotificationRepository notificationRepository, ILogger<BaseJob> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }
        public async Task Invoke()
        {
            _logger.LogInformation("Task {0} start working!", GetType().Name);
            try
            {
                await ExecuteAsync();
                _logger.LogInformation("Task {0} successfully completed!", GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Task {0} has error!", GetType().Name);
                await _notificationRepository.CreateAsync(new Models.db.Notification { Message = ex.Message, NotificationType = Models.enums.NotificationType.Error });
            }
        }

        protected abstract Task ExecuteAsync();
    }
}
