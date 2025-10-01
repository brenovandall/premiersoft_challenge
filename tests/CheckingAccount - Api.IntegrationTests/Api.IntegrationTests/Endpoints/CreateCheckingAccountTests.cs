using Application.CheckingAccount.Commands.CreateCheckingAccount;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;

namespace Api.IntegrationTests.Endpoints
{
    public class CreateCheckingAccountTests : IClassFixture<WebApiApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private string _connectionString;

        private const string Cpf = "24048134043";

        public CreateCheckingAccountTests(WebApiApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _connectionString = factory.Configuration.GetConnectionString("DbConnection")!;
        }

        [Fact]
        public async Task POST_ShouldReturn200_WhenValidRequest()
        {
            var request = new { Cpf, Password = "aaa" };

            var response = await _client.PostAsJsonAsync("/v1/checkingAccount/createAccount", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<CreateCheckingAccountResult>();

            Assert.NotNull(result);
            Assert.NotEqual(0, result.AccountNumber);
        }

        [Fact]
        public async Task POST_ShouldReturn400_WhenCpfIsInvalid()
        {
            var request = new { Cpf = "10637012955", Password = "aaa" };

            var response = await _client.PostAsJsonAsync("/v1/checkingAccount/createAccount", request);
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("INVALID_DOCUMENT", body);
        }

        public void Dispose()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM contacorrente WHERE nome = '{Cpf}'";
            cmd.ExecuteNonQuery();
        }
    }
}
