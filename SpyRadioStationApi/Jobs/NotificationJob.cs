
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Exceptions;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.enums;

namespace SpyRadioStationApi.Jobs
{
    public class NotificationJob : BaseJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Telegram _configuration;
        public NotificationJob(INotificationRepository notificationRepository, 
            IHttpClientFactory httpClientFactory,
            IOptions<Telegram> configuration) : base(notificationRepository)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }
     
        protected override async Task ExecuteAsync()
        {
            
            var notifications = await _notificationRepository.GetByStatusAsync(Status.New);

            if (!notifications.Any())
                return;


            var telegram = $"https://api.telegram.org/bot{TOKEN}/sendMessage?chat_id={chat_id}&text={message}";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, telegram);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(httpRequestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new DataRequestException($"{response.StatusCode} : No data received!");
            }

            await _notificationRepository.UpdateAsync(Status.Processed, notifications.Select(x => x.Id).ToArray());
        }
    }
}
