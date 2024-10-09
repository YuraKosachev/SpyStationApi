using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
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

            using var connection = new SqliteConnection(_configuration.DatabaseName);

            await connection.ExecuteAsync("""
                    CREATE TABLE IF NOT EXISTS Migrations (Id INTEGER PRIMARY KEY AUTOINCREMENT,Name VARCHAR(300) NOT NULL, Date DATE NOT NULL);
                """);

            if (!files.Any()) return;

            var migrations = (await connection.QueryAsync<Migration>("SELECT Id, Name, Date FROM Migrations;"))
                .Select(m => m.Name).ToArray();

            StringBuilder builder = new StringBuilder();

            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                if (migrations.Contains(name))
                    continue;

                builder.Append(File.ReadAllText(file));
            }

            var query = builder.ToString();

            if(!string.IsNullOrEmpty(query))
                await connection.ExecuteAsync(query);
        }


    }
}
