using FastEndpoints;
using SpyRadioStationApi.Contracts.Request;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;

namespace SpyRadioStationApi.Endpoints
{
    public class PostValidationEncodeEndpoint : Endpoint<ValidationRequest, ValidationResponse>
    {
        private readonly IRadiogramRepository _radiogramRepository;

        public PostValidationEncodeEndpoint(IRadiogramRepository radiogramRepository)
        {
            _radiogramRepository = radiogramRepository;
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

            var message = isValid
                ? "The message was successfully decrypted!!"
                : """
                Validation of the message failed, the message may have expired or it was incorrectly decrypted. Try again!!
                """;

            await SendOkAsync(new ValidationResponse(isValid, message), ct);
        }

    }
}
