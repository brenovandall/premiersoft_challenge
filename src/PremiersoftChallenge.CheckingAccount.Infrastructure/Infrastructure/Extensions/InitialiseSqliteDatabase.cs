using Microsoft.Data.Sqlite;

namespace Infrastructure.Extensions
{
    public static class InitialiseSqliteDatabase
    {
        public static async Task CreateTablesAsync()
        {
            using var connection = new SqliteConnection(ConnectionStringBuilder.GetConnectionString());
            await connection.OpenAsync();

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
            await command.ExecuteNonQueryAsync();
        }
    }
}
