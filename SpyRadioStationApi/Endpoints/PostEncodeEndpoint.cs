using FastEndpoints;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Contracts.Request;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;
using SpyRadioStationApi.Models;
using System.Configuration;

namespace SpyRadioStationApi.Endpoints
{
    public class PostEncodeEndpoint : Endpoint<CodeRequest, CodeResponse>
    {
        private readonly ICodeService _codeService;
        private readonly IKeyCodeMachineRepository _codeMachineRepository;
        private readonly Access _access;
        public PostEncodeEndpoint(ICodeService codeService,
            IKeyCodeMachineRepository codeMachineRepository,
            IOptions<Access> access)
        {
            _codeMachineRepository = codeMachineRepository;
            _codeService = codeService;
            _access = access?.Value ?? throw new ArgumentNullException(nameof(access));
        }
        public override void Configure()
        {
            Post("/spy/enigma/encode");
            AllowAnonymous();
        }
        public override async Task HandleAsync(CodeRequest req, CancellationToken ct)
        {
            var accessKey = HttpContext.Request?.Headers["SPY-Access"];

            if (!accessKey.Equals(_access.Key))
                await SendErrorsAsync(401, ct);

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
