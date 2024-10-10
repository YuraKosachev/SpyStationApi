using FastEndpoints;
using SpyRadioStationApi.Contracts.Request;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;
using SpyRadioStationApi.Models;

namespace SpyRadioStationApi.Endpoints
{
    public class PostEncodeEndpoint : Endpoint<CodeRequest, CodeResponse>
    {
        private readonly ICodeService _codeService;
        private readonly IKeyCodeMachineRepository _codeMachineRepository;

        public PostEncodeEndpoint(ICodeService codeService,
            IKeyCodeMachineRepository codeMachineRepository)
        {
            _codeMachineRepository = codeMachineRepository;
            _codeService = codeService;
        }
        public override void Configure()
        {
            Post("/spy/enigma/encode");
            AllowAnonymous();
        }
        public override async Task HandleAsync(CodeRequest req, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(req?.message))
                await SendErrorsAsync();

            var key = await _codeMachineRepository.GetByDayAsync(DateTime.Now.Day);

            var setting = new Settings
            {
                FastRotorLetter = key?.Fast ?? 'A',
                MediumRotorLetter = key?.Medium ?? 'A',
                SlowRotorLetter = key?.Slow ?? 'A',
                Plugboard = key?.Plugboard
            };

            var code = _codeService.GetCodeMessage(req.message, setting);

            await SendOkAsync(new CodeResponse(code, DateTime.Now), ct);
        }

    }
}
