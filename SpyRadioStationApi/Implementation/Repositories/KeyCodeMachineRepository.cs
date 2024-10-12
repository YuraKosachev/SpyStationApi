using Dapper;
using SpyRadioStationApi.Implementation.Mapping;
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Implementation.Repositories
{
    public class KeyCodeMachineRepository : IKeyCodeMachineRepository
    {
        private readonly IDatabaseContext _dbContext;

        public KeyCodeMachineRepository(IDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Key> GetByDayAsync(int day)
        {
            using var connection = _dbContext.CreateConnection();

            var item = await connection.QueryFirstAsync<dynamic>("""
                SELECT Id, Slow, Medium, Fast, Day, Plugboard, CreateAt FROM Keys
                WHERE Day = @day
                """, new { day });

            return Mapper.ToKey(item);
        }
    }
}