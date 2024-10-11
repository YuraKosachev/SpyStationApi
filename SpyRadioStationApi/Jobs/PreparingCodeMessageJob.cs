using Coravel.Invocable;
using HtmlAgilityPack;
using SpyRadioStationApi.Exceptions;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;
using SpyRadioStationApi.Models;
using SpyRadioStationApi.Models.db;
using SpyRadioStationApi.Models.enums;

namespace SpyRadioStationApi.Jobs
{
    public class PreparingCodeMessageJob : BaseJob
    {
        private readonly ICodeService _codeService;
        private readonly IRadiogramRepository _radiogramRepository;
        private readonly IKeyCodeMachineRepository _keyProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        public PreparingCodeMessageJob(ICodeService codeService,
            IKeyCodeMachineRepository keyProvider,
            IRadiogramRepository radiogramRepository,
            IHttpClientFactory httpClientFactory,
            INotificationRepository notificationRepository,
            ILogger<BaseJob> logger):base(notificationRepository, logger)
        {
            _keyProvider = keyProvider;
            _codeService = codeService;
            _radiogramRepository = radiogramRepository;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync()
        {
            var messages = await TryToGetMessage();
            var key = await _keyProvider.GetByDayAsync(DateTime.Now.Day);
            var saved = (await _radiogramRepository.GetAllAsync())?.Select(x => x.Message).ToList();

            var setting = new Settings
            {
                FastRotorLetter = key?.Fast ?? 'A',
                MediumRotorLetter = key?.Medium ?? 'A',
                SlowRotorLetter = key?.Slow ?? 'A',
                Plugboard = key?.Plugboard
            };

            var radiograms = new List<Radiogram>();
            foreach (var message in messages)
            {
                var checkMessage = message?.Trim().ToUpper();

                if (string.IsNullOrEmpty(checkMessage) || (saved != null && saved.Contains(checkMessage)))
                    continue;

                try
                {
                    var encode = _codeService.GetCodeMessage(checkMessage, setting);
                    radiograms.Add(new Radiogram { Message = checkMessage, Encode = encode, CreateAt = DateTime.Now });
                }
                catch (Exception ex)
                {
                    await _notificationRepository.CreateAsync(new Notification { Message = $"Incorrect text's letter - {checkMessage}", NotificationType = NotificationType.Error });
                }
            }

            //save 
            for (int i = 0; ; i++) 
            {
                var portion = radiograms.Skip(i * 30).Take(30).ToList();
                if (portion == null || !portion.Any())
                    break;
                await _radiogramRepository.CreateRangeAsync(portion);
            }

            await _notificationRepository.CreateAsync(new Notification { Message = $"The parsing has ended. Count new radiograms preparing {radiograms.Count}",
                NotificationType = NotificationType.Info }); 

        }
        private async Task<IList<string>> TryToGetMessage()
        {

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://edition.cnn.com/world");
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(httpRequestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new DataRequestException($"{response.StatusCode} : No data received!");
            }
            var html = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(html);

            List<string> nodes = new List<string>();

            foreach (HtmlNode node in htmlSnippet.DocumentNode.SelectNodes("//span[@class='container__headline-text']"))
            {
                nodes.Add(node.InnerText);
            }

            return nodes;
        }
    }
}
