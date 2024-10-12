using FastEndpoints;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Contracts.Request;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.enums;

namespace SpyRadioStationApi.Endpoints
{
    public class PostNotificationEndpoint : Endpoint<NotificationRequest, NotificationResponse>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly Access _access;

        public PostNotificationEndpoint(INotificationRepository notificationRepository,
            IOptions<Access> access)
        {
            _notificationRepository = notificationRepository;
            _access = access?.Value ?? throw new ArgumentNullException(nameof(access));
        }

        public override void Configure()
        {
            Post("/spy/notification/message");
            AllowAnonymous();
        }

        public override async Task HandleAsync(NotificationRequest req, CancellationToken ct)
        {
            var accessKey = HttpContext.Request?.Headers["SPY-Access"];

            if (!accessKey.Equals(_access.Key))
                await SendErrorsAsync(401, ct);

            if (string.IsNullOrEmpty(req?.message))
                await SendErrorsAsync();

            await _notificationRepository.CreateAsync(new Models.db.Notification { Message = req.message, NotificationType = (NotificationType)req.type });

            await SendOkAsync(new NotificationResponse("Successfully created!"), ct);
        }
    }
}