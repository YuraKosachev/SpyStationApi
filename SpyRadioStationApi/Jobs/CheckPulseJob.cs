using Coravel.Invocable;

namespace SpyRadioStationApi.Jobs
{
    public class CheckPulseJob: IInvocable
    {
        private readonly ILogger<CheckPulseJob> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public CheckPulseJob(IHttpClientFactory httpClientFactory,
            ILogger<CheckPulseJob> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Invoke()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://spystationapi.onrender.com/spy/pulse/ping");
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Pulse checked! Heart is beating status code: {0}", response.StatusCode);
            }
        }
    }
}
