using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;

namespace SpyRadioStationApi.Endpoints
{
    public record InformationResponse(string access, string token, bool isdbFolderExists, string db, string diff, int rows);
    public class GetTestInformationEndpoint : EndpointWithoutRequest<InformationResponse>
    {
        private readonly DbConfiguration _dbConfiguration;
        private readonly Telegram _telegram;
        private readonly Access _access;
        public GetTestInformationEndpoint(IOptions<DbConfiguration> dbConfiguration,
            IOptions<Telegram> telegram,
            IOptions<Access> access)
        {
            _dbConfiguration = dbConfiguration?.Value;
            _telegram = telegram?.Value;
            _access = access?.Value;
        }
        public override void Configure()
        {
            Get("/spy/information");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            using var connection = new SqliteConnection(_dbConfiguration.DatabaseName);
            var rows = await connection.ExecuteAsync("SELECT * FROM Migrations");

            var i = Directory.Exists("db");
           await SendOkAsync(new InformationResponse(_access?.Key, _telegram?.Token, i, _dbConfiguration.DatabaseName, _dbConfiguration.Diff, rows));
            
        }
    }
}
