using FastEndpoints;

namespace SpyRadioStationApi.Endpoints
{
    public record PulseRequest(string message);

    public class GetPulseEndpoint : EndpointWithoutRequest<PulseRequest>
    {
        public override void Configure()
        {
            Get("/spy/pulse/ping");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await SendOkAsync(new PulseRequest($"pong::{DateTime.Now}"));
        }
    }
}