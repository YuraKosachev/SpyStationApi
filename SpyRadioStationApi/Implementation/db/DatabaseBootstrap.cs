using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Models.db;
using System.Text;

namespace SpyRadioStationApi.Implementation.db
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DbConfiguration _configuration;

        public DatabaseBootstrap(IOptions<DbConfiguration> configuration)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Setup()
        {
            var files = Directory.GetFiles(_configuration.Diff)
                .Where(file => file.EndsWith(".sql"))
                .ToList();

            if (!Directory.Exists(_configuration.DbFolder))
                Directory.CreateDirectory(_configuration?.DbFolder);

            using var connection = new SqliteConnection(_configuration.DatabaseName);

            await connection.ExecuteAsync("""
                    CREATE TABLE IF NOT EXISTS Migrations (Id INTEGER PRIMARY KEY AUTOINCREMENT,Name VARCHAR(300) NOT NULL, Date DATE NOT NULL);
                """);

            if (!files.Any()) return;

            var migrations = (await connection.QueryAsync<Migration>("SELECT Id, Name, Date FROM Migrations;"))
                .Select(m => m.Name).ToArray();

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

            var query = builder.ToString();
            var queryMigration = migrationBuilder.ToString();

            if (!string.IsNullOrEmpty(query))
                await connection.ExecuteAsync(query);

            if (!string.IsNullOrEmpty(queryMigration))
                await connection.ExecuteAsync(queryMigration);
        }


    }
}
