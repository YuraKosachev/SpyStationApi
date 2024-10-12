using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Npgsql;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Models.db;

namespace SpyRadioStationApi.Context
{
    public class SpyDbContext : IDatabaseContext
    {
        private DbConfiguration _dbConfiguration;

        public SpyDbContext(IOptions<DbConfiguration> dbConfiguration)
        {
            _dbConfiguration = dbConfiguration.Value;
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_dbConfiguration.ConnectionString);


        private async Task<IList<Migration>> GetMigrationsAsync()
        {
            using var connection = CreateConnection();
            string commandText = $"SELECT id, name, date FROM Migrations;";
            var migrations = (await connection.QueryAsync<Migration>(commandText)).ToList();

            return migrations;
        }

        private async Task ExecuteQueriesAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
                return;
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query);
        }

        public async Task Init()
        {
            var files = Directory.GetFiles(_dbConfiguration.Diff).Where(file => file.EndsWith(".sql")).ToList();

            var createMigrationDbQuery = """
                    CREATE TABLE IF NOT EXISTS Migrations (Id SERIAL PRIMARY KEY,Name VARCHAR(300) NOT NULL, Date DATE NOT NULL);
                """;
            
            await ExecuteQueriesAsync(createMigrationDbQuery);

            if (!files.Any()) return;

            var migrations = (await GetMigrationsAsync()).Select(m => m.Name).ToArray();

            StringBuilder builder = new StringBuilder();
            StringBuilder migrationBuilder = new StringBuilder();
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                if (migrations.Contains(name))
                    continue;

                migrationBuilder.Append($"INSERT INTO Migrations(Name,Date) VALUES('{name}','{DateTime.Now}');");
                builder.Append(File.ReadAllText(file));
            }

            await ExecuteQueriesAsync(builder.ToString());
            await ExecuteQueriesAsync(migrationBuilder.ToString());

        }
    }
}
