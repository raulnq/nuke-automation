using DbUp;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Core.Migrator
{
    public class DbMigrator
    {
        private readonly string _connectionString;

        private readonly string _schema;

        public DbMigrator(Assembly assembly)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddEnvironmentVariables()
                                .AddUserSecrets(assembly)
                                .Build();

            _connectionString = configuration["DbConnectionString"];

            _schema = configuration["DbSchema"];

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Missing DbConnection configuration");
            }

            if (string.IsNullOrEmpty(_schema))
            {
                _schema = "dbo";
            }
        }

        public void Migrate()
        {
            var _assembly = Assembly.GetEntryAssembly();

            EnsureDatabase.For.SqlDatabase(_connectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(_connectionString, schema: _schema)
                    .WithScriptsEmbeddedInAssembly(_assembly)
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new DbMigrationException(result.Error);
            }
        }
    }
}