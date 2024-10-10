using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Implementation.Mapping;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Models.db;
using SpyRadioStationApi.Models.enums;

namespace SpyRadioStationApi.Implementation.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DbConfiguration _configuration;

        public NotificationRepository(IOptions<DbConfiguration> configuration)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task CreateAsync(Notification notification)
        {
            using var connection = new SqliteConnection(_configuration.DatabaseName);

            await connection.QueryAsync("""
                INSERT INTO Notifications(Message, Status, Type, CreateAt ) VALUES(@message, @status, @type, @date)
                """, new { message = notification.Message, status = (int)Status.New, type = (int)notification.NotificationType, date = DateTime.Now });
        }

        public async Task DeleteByStatusAsync(Status status)
        {
            using var connection = new SqliteConnection(_configuration.DatabaseName);

            await connection.QueryAsync("""
                DELETE FROM Notifications WHERE Status = @status
                """, new { status = (int)status });
        }

        public async Task<IList<Notification>> GetAllAsync()
        {
            using var connection = new SqliteConnection(_configuration.DatabaseName);

            var list = await connection.QueryAsync<dynamic>("""
                SELECT Id, Message, Status, Type, CreateAt FROM Notifications
                """, new { });

            var result = new List<Notification>();
            foreach (var notification in list)
            {
                result.Add(Mapper.ToNotification(notification));
            }
            return result;
        }

        public async Task<IList<Notification>> GetByStatusAsync(Status status)
        {
            using var connection = new SqliteConnection(_configuration.DatabaseName);

            var list = await connection.QueryAsync<dynamic>("""
                SELECT Id, Message, Status, Type, CreateAt FROM Notifications WHERE Status = @status
                """, new { status = (int)status });

            var result = new List<Notification>();
            foreach (var notification in list)
            {
                result.Add(Mapper.ToNotification(notification));
            }
            return result;
        }

        public async Task UpdateAsync(Status status, params int[] ids)
        {
            using var connection = new SqliteConnection(_configuration.DatabaseName);

            var predicate = string.Join(" OR ", ids.Select(x => $"Id={x}").ToArray());
            var query = $"UPDATE Notifications SET Status = {(int)status} WHERE {predicate}";

            await connection.QueryAsync(query);
        }
    }
}
