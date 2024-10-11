using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Contracts.Response;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;
using System.Runtime.InteropServices;

namespace SpyRadioStationApi.Endpoints
{
    public record InformationResponse(string access, string token, bool isdbFolderExists, string db, string diff, string rows, dynamic info, string files);
    public record TableInformation(string tables);
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
            var rows = (await connection.QueryAsync<dynamic>("SELECT Name FROM Migrations"))?.Select(x=>(string)x.Name).ToList();
            var information = await connection.QueryAsync<dynamic>("SELECT name FROM sqlite_master");//)?.Select(x => (string)x.Name).ToList();
            var i = Directory.Exists("db");
            var diff = Directory.Exists("diff");
            string files = "";
            if (diff) 
            {
                var list = Directory.GetFiles("diff");
                files = string.Join(",", list);
            }

            await SendOkAsync(new InformationResponse(_access?.Key, _telegram?.Token, i, _dbConfiguration.DatabaseName, _dbConfiguration.Diff, string.Join(",",rows), information, files));
            
        }
    }
}
