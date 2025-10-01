using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;

namespace Api.IntegrationTests.Endpoints
{
    public class InactivateAccountTests : IClassFixture<WebApiApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private string _connectionString;
        private string _token = default!;

        public InactivateAccountTests(WebApiApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _connectionString = factory.Configuration.GetConnectionString("DbConnection")!;
        }

        private const string Identifier = "71300226048";

        [Fact]
        public async Task POST_ShouldReturnNoContent_WhenAuthenticated()
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var request = new { Password = "aaa" };
            var response = await _client.PostAsJsonAsync("/v1/checkingAccount/inactivate", request);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task POST_ShouldReturnUnauthorized_WhenNotAuthenticated()
        {
            var request = new { Password = "aaa" };

            var response = await _client.PostAsJsonAsync("/v1/checkingAccount/inactivate", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task POST_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var request = new { Password = "bbb" };
            var response = await _client.PostAsJsonAsync("/v1/checkingAccount/inactivate", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        public async Task InitializeAsync()
        {
            // este cenario esta no banco de testes
            var loginRequest = new { Identifier, Password = "aaa" };
            var loginResponse = await _client.PostAsJsonAsync("/v1/checkingAccount/login", loginRequest);
            _token = await loginResponse.Content.ReadFromJsonAsync<string>() ?? "";
        }

        public async Task DisposeAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE contacorrente SET ativo = 1 WHERE nome = '{Identifier}'";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
