using FastEndpoints;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Services;
namespace SpyRadioStationApi.Endpoints
{
    public class GetEnigmaEndpoint : EndpointWithoutRequest<CodeResponse>
    {
        private readonly ICodeService _codeService;

        public GetEnigmaEndpoint(ICodeService codeService)
        {
            _codeService = codeService;
        }
        public override void Configure()
        {
            Get("/spy/enigma/message");

            AllowAnonymous();
            //Summary(s =>
            //{
            //    s.Summary = "Summary";
            //    s.Description = "My endpoint description";
            //    s.Params["message"] = "message";
            //});
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var decode = _codeService.GetCodeMessage("JORZV BF VIWMAKT");
            await SendOkAsync(new CodeResponse(decode));
        }
    }
}
