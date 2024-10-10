
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Exceptions;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.enums;
using System.Text;

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

        private void FillMessage(string title, IList<string> messages, StringBuilder builder)
        {

            if (!messages.Any())
                return;

            if (!string.IsNullOrEmpty(title))
                builder.AppendLine($"{title}\n");

            foreach (var message in messages)
            {
                if (!string.IsNullOrEmpty(message))
                    builder.Append($"`{message}`\n");
            }
        }

        protected override async Task ExecuteAsync()
        {

            var notifications = await _notificationRepository.GetByStatusAsync(Status.New);

            if (!notifications.Any())
                return;
            //Errors
            var errors = notifications.Where(not => not.NotificationType == NotificationType.Error).Select(not=>not.Message).ToList();
            var infos = notifications.Where(not => not.NotificationType == NotificationType.Info).Select(not => not.Message).ToList();
            var warnings = notifications.Where(not => not.NotificationType == NotificationType.Warning).Select(not => not.Message).ToList();
            var winners = notifications.Where(not => not.NotificationType == NotificationType.Winner).Select(not => not.Message).ToList();

            StringBuilder builder = new StringBuilder();

            FillMessage("🚨 *Spy station errors:*", errors, builder);
            FillMessage("ℹ️ *Spy station informations:*", infos, builder);
            FillMessage("⚠️ *Spy station warnings:*", warnings, builder);
            FillMessage("✨🎁🏆 *Spy station winners:*", winners, builder);

            var telegram = $"https://api.telegram.org/bot{_configuration.Token}/sendMessage?parse_mode=MarkdownV2&chat_id={_configuration.ChatId}&text={builder.ToString()}";
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
