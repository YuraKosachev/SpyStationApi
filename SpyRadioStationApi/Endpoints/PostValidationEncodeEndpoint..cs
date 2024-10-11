using FastEndpoints;
using SpyRadioStationApi.Contracts.Request;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;
using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Endpoints
{
    public class PostValidationEncodeEndpoint : Endpoint<ValidationRequest, ValidationResponse>
    {
        private readonly IRadiogramRepository _radiogramRepository;
        private readonly INotificationRepository _notificationRepository;

        public PostValidationEncodeEndpoint(IRadiogramRepository radiogramRepository,
            INotificationRepository notificationRepository)
        {
            _radiogramRepository = radiogramRepository;
            _notificationRepository = notificationRepository;
        }
        public override void Configure()
        {
            Post("/spy/enigma/validation");
            AllowAnonymous();
        }
        public override async Task HandleAsync(ValidationRequest req, CancellationToken ct)
        {
            if (req?.encode == null || req?.decode == null)
                await SendErrorsAsync();
            var isValid = await _radiogramRepository.ValidationAsync(req.encode, req.decode);

            if (isValid)
            {
                HttpContext.Request.Headers.TryGetValue("True-Client-Ip", out var remoteIp);
                var notification = $"{remoteIp} - successfully decrypted code. Code is {req.encode}";
                await _notificationRepository.CreateAsync(new Notification { Message = notification, NotificationType = Models.enums.NotificationType.Winner });
            }

            var message = isValid
                ? "The message was successfully decrypted!!"
                : """
                Validation of the message failed, the message may have expired or it was incorrectly decrypted. Try again!!
                """;

            await SendOkAsync(new ValidationResponse(isValid, message), ct);
        }

    }
}
