
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Implementation.Mapping;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.db;


namespace SpyRadioStationApi.Implementation.Repositories
{
    public class KeyCodeMachineRepository : IKeyCodeMachineRepository
    {
        private readonly DbConfiguration _configuration;

        public KeyCodeMachineRepository(IOptions<DbConfiguration> configuration)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }


        public async Task<Key> GetByDayAsync(int day)
        {
            using var connection = new SqliteConnection(_configuration.DatabaseName);
          
            var item = await connection.QueryFirstAsync<dynamic>("""
                SELECT Id, Slow, Medium, Fast, Day, Plugboard, CreateAt FROM Keys 
                WHERE Day = @day
                """, new { day });

            return Mapper.ToKey(item);
        }
    }
}
