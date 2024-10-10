using FastEndpoints;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;
namespace SpyRadioStationApi.Endpoints
{
    public class GetEncodeMessageEndpoint : EndpointWithoutRequest<CodeResponse>
    {
        private readonly ICodeService _codeService;
        private readonly IRadiogramRepository _radiogramRepository;

        public GetEncodeMessageEndpoint(ICodeService codeService,
            IRadiogramRepository radiogramRepository)
        {
            _codeService = codeService;
            _radiogramRepository = radiogramRepository;
        }
        public override void Configure()
        {
            Get("/spy/enigma/message");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var telegrams = await _radiogramRepository.GetAllAsync();

            if (telegrams != null && telegrams.Any())
            {
                var telegram = telegrams[new Random().Next(0, telegrams.Count - 1)];
                await SendOkAsync(new CodeResponse(telegram?.Encode, telegram.CreateAt));
            }

            else
                await SendErrorsAsync();
        }
    }
}
