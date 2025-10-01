using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Api.IntegrationTests.Endpoints
{
    public class MakeTransactionTests : IClassFixture<WebApiApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private string _connectionString;

        private const string FirstAccount = "04224725061";
        private const string SecondAccount = "74123820042";

        public MakeTransactionTests(WebApiApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _connectionString = factory.Configuration.GetConnectionString("DbConnection")!;
        }

        [Fact]
        public async Task POST_ShouldReturnNoContent_WhenTransactionIsValid()
        {
            var id = Guid.NewGuid().ToString();
            var request = new
            {
                RequestId = id,
                AccountNumber = (long?)null,
                Value = 100.0,
                TransactionFlow = "C"
            };

            var response = await _client.PostAsJsonAsync("/v1/transaction/perform", request);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.True(await TransactionWasComitted(id));
        }

        [Fact]
        public async Task POST_ShouldReturnBadRequest_WhenRequestIdIsInvalid()
        {
            var id = "6sgxjb1y18hs17s781s";
            var request = new
            {
                RequestId = id,
                AccountNumber = (long?)null,
                Value = 100.0,
                TransactionFlow = "C"
            };

            var response = await _client.PostAsJsonAsync("/v1/transaction/perform", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(await TransactionWasComitted(id));
        }

        [Fact]
        public async Task POST_ShouldReturnInvalidAccount_WhenAccountDoesNotExist()
        {
            var id = Guid.NewGuid().ToString();
            var request = new
            {
                RequestId = id,
                AccountNumber = 9999999,
                Value = 100.0,
                TransactionFlow = "C"
            };

            var response = await _client.PostAsJsonAsync("/v1/transaction/perform", request);
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("INVALID_ACCOUNT", body);
            Assert.False(await TransactionWasComitted(id));
        }

        [Fact]
        public async Task POST_ShouldReturnBadRequest_WhenDebitInDifferentAccount()
        {
            var id = Guid.NewGuid().ToString();
            var request = new
            {
                RequestId = id,
                AccountNumber = SecondAccount,
                Value = 50.0,
                TransactionFlow = "D"
            };

            var response = await _client.PostAsJsonAsync("/v1/transaction/perform", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(await TransactionWasComitted(id));
        }

        [Fact]
        public async Task POST_ShouldBeIdempotent_WhenRequestIdIsRepeated()
        {
            var id = Guid.NewGuid().ToString();
            var request = new
            {
                RequestId = id,
                AccountNumber = (long?)null,
                Value = 200.0,
                TransactionFlow = "C"
            };

            _client.DefaultRequestHeaders.Remove("Idempotency-Key");
            _client.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString());

            var response1 = await _client.PostAsJsonAsync("/v1/transaction/perform", request);
            var response2 = await _client.PostAsJsonAsync("/v1/transaction/perform", request);

            Assert.Equal(HttpStatusCode.NoContent, response1.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);
            Assert.True(await TransactionWasComitted(id));
        }

        private async Task<bool> TransactionWasComitted(string id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var sql = "SELECT idmovimento Result FROM movimento WHERE idmovimento = @id";
            var param = new { id = id.ToUpper() };

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, param);

            return !string.IsNullOrEmpty(result);
        }

        public async Task InitializeAsync()
        {
            // este cenario esta no banco de testes
            var loginRequest = new { Identifier = FirstAccount, Password = "aaa" };
            var loginResponse = await _client.PostAsJsonAsync("/v1/checkingAccount/login", loginRequest);
            var token = await loginResponse.Content.ReadFromJsonAsync<string>() ?? "";

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task DisposeAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM movimento;DELETE FROM idempotencia";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
