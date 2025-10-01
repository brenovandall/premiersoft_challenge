using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Api.IntegrationTests
{
    public class WebApiApplicationFactory : WebApplicationFactory<Program>
    {
        public IConfiguration Configuration { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                Configuration = new ConfigurationBuilder().AddJsonFile("integrationsettings.json").Build();
                config.AddConfiguration(Configuration);
            });
            builder.ConfigureTestServices(services =>
            {
                EnsureDatabaseCreated();
            });
        }

        private void EnsureDatabaseCreated()
        {
            using var connection = new SqliteConnection(Configuration.GetConnectionString("DbConnection"));
            connection.Open();

            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS contacorrente (
					idcontacorrente TEXT(37) PRIMARY KEY,
					numero INTEGER(10) NOT NULL UNIQUE,
					nome TEXT(100) NOT NULL,
					ativo INTEGER(1) NOT NULL default 0,
					senha TEXT(100) NOT NULL,
					salt TEXT(100) NOT NULL,
					CHECK (ativo in (0,1))
				);

				CREATE TABLE IF NOT EXISTS movimento (
					idmovimento TEXT(37) PRIMARY KEY,
					idcontacorrente TEXT(37) NOT NULL,
					datamovimento TEXT(25) NOT NULL,
					tipomovimento TEXT(1) NOT NULL,
					valor REAL NOT NULL,
					CHECK (tipomovimento in ('C','D')),
					FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
				);

				CREATE TABLE IF NOT EXISTS idempotencia (
					chave_idempotencia TEXT(37) PRIMARY KEY,
					requisicao TEXT(1000),
					resultado TEXT(1000)
				);
            ";

            using var command = connection.CreateCommand();
            command.CommandText = createTableSql;
            command.ExecuteNonQuery();
        }
    }
}
