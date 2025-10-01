using System.Net;
using System.Net.Http.Json;

namespace Api.IntegrationTests.Endpoints
{
    public class LoginCheckingAccountTests : IClassFixture<WebApiApplicationFactory>
    {
        private readonly HttpClient _client;

        public LoginCheckingAccountTests(WebApiApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private const string Identifier = "04224725061";
        private const string Password = "aaa";

        [Fact]
        public async Task POST_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // este cenario esta no banco de dados de testes
            var loginRequest = new { Identifier, Password };
            var loginResponse = await _client.PostAsJsonAsync("/v1/checkingAccount/login", loginRequest);
            var token = await loginResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task POST_ShouldFail_WhenPasswordIsInvalid()
        {
            var loginRequest = new { Identifier, Password = "bbb" };
            var loginResponse = await _client.PostAsJsonAsync("/v1/checkingAccount/login", loginRequest);
            var body = await loginResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
            Assert.Contains("USER_UNAUTHORIZED", body);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenAccountDoesNotExist()
        {
            var loginRequest = new { Identifier = "", Password = "" };
            var loginResponse = await _client.PostAsJsonAsync("/v1/checkingAccount/login", loginRequest);
            var body = await loginResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
            Assert.Contains("USER_UNAUTHORIZED", body);
        }
    }
}
