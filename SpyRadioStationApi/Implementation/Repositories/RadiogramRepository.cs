using Dapper;
using SpyRadioStationApi.Implementation.Mapping;
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.db;
using System.Text;

namespace SpyRadioStationApi.Implementation.Repositories
{
    public class RadiogramRepository : IRadiogramRepository
    {
        private readonly IDatabaseContext _dbContext;

        public RadiogramRepository(IDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(Radiogram radiogram)
        {
            using var connection = _dbContext.CreateConnection();

            await connection.QueryAsync("""
                INSERT INTO Radiograms(Message, Encode, CreateAt ) VALUES(@message, @encode, @date);
                """, new { message = radiogram.Message, encode = radiogram.Encode, date = radiogram.CreateAt });
        }

        public async Task CreateRangeAsync(IList<Radiogram> radiograms)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Radiogram radiogram in radiograms)
            {
                builder.Append($"INSERT INTO Radiograms(Message, Encode, CreateAt ) VALUES('{radiogram.Message}', '{radiogram.Encode}', '{radiogram.CreateAt}');");
            }
            using var connection = _dbContext.CreateConnection();

            await connection.QueryAsync(builder.ToString());
        }

        public async Task DeleteUntilAsync(DateTime date)
        {
            using var connection = _dbContext.CreateConnection();

            await connection.QueryAsync("""
                DELETE FROM Radiograms WHERE CreateAt < @date
                """, new { date });
        }

        public async Task<IList<Radiogram>> GetAllAsync()
        {
            using var connection = _dbContext.CreateConnection();

            var list = await connection.QueryAsync<dynamic>("""
                SELECT Id, Message, Encode, CreateAt FROM Radiograms
                """, new { });

            var result = new List<Radiogram>();
            foreach (var radiogram in list)
            {
                result.Add(Mapper.ToRadiogram(radiogram));
            }
            return result;
        }

        public async Task<Radiogram> GetByIdAsync(int id)
        {
            using var connection = _dbContext.CreateConnection();

            var item = await connection.QueryFirstOrDefaultAsync<dynamic>("""
                SELECT Id, Message, Encode, CreateAt FROM Radiograms
                WHERE Id = @id
                """, new { id });

            return Mapper.ToRadiogram(item);
        }

        public async Task<Radiogram> GetLastByDateAsync()
        {
            using var connection = _dbContext.CreateConnection();

            var item = await connection.QueryFirstOrDefaultAsync<dynamic>("""
                SELECT Id, Message, Encode, CreateAt FROM Radiograms
                ORDER BY CreateAt desc
                LIMIT 1
                """);

            return Mapper.ToRadiogram(item);
        }

        public async Task<bool> ValidationAsync(string encode, string decode)
        {
            using var connection = _dbContext.CreateConnection();
            var item = await connection.QueryFirstOrDefaultAsync<dynamic>("""
                SELECT Id, Message, Encode, CreateAt FROM Radiograms
                WHERE Encode = @encode AND Message = @decode
                """, new { encode, decode });

            return item != null;
        }
    }
}