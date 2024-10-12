using Dapper;
using FastEndpoints;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Interfaces.db;

namespace SpyRadioStationApi.Endpoints
{
    public record InformationResponse(HeaderInformation header, string access, string token, bool isdbFolderExists, string db, string diff, string rows, string files);
    public record HeaderInformation(IHeaderDictionary header, string ip);

    public class GetTestInformationEndpoint : EndpointWithoutRequest<InformationResponse>
    {
        private readonly DbConfiguration _dbConfiguration;
        private readonly IDatabaseContext _databaseContext;
        private readonly Telegram _telegram;
        private readonly Access _access;

        public GetTestInformationEndpoint(IOptions<DbConfiguration> dbConfiguration,
            IDatabaseContext databaseContext,
            IOptions<Telegram> telegram,
            IOptions<Access> access)
        {
            _dbConfiguration = dbConfiguration?.Value;
            _telegram = telegram?.Value;
            _access = access?.Value;
            _databaseContext = databaseContext;
        }

        public override void Configure()
        {
            Get("/spy/information");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var accessKey = HttpContext.Request?.Headers["SPY-Access"];

            if (!accessKey.Equals(_access.Key))
                await SendErrorsAsync(401, ct);

            using var connection = _databaseContext.CreateConnection();
            var rows = (await connection.QueryAsync<dynamic>("SELECT Name FROM Migrations"))?.Select(x => (string)x.Name).ToList();
           
            var i = Directory.Exists("db");
            var diff = Directory.Exists("diff");
            string files = "";

            if (diff)
            {
                var list = Directory.GetFiles("diff");
                files = string.Join(",", list);
            }

            var headers = HttpContext.Request.Headers;
            HttpContext.Request.Headers.TryGetValue("True-Client-Ip", out var remoteIp);
            var hesd = new HeaderInformation(headers, remoteIp);
            await SendOkAsync(new InformationResponse(hesd, _access?.Key, _telegram?.Token, i, _dbConfiguration.ConnectionString, _dbConfiguration.Diff, string.Join(",", rows), files));
        }
    }
}