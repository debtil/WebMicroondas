using Newtonsoft.Json;
using System.Text;
using WebMicroondas.Models;
using WebMicroondasTests.Helpers;

namespace WebMicroondasTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly HttpClient _client;

        public AuthControllerTests()
        {
            _client = ServerHelperTests.GetTestServerClient().Result;
        }

        [Fact]
        public async Task Login_Should_Return_Token_When_Valid_Credentials()
        {
            // Arrange
            var loginRequest = new LoginRequest("admin", "microwave123");

            var jsonRequest = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se a resposta foi sucesso
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", responseString); // Verifica se o token está presente na resposta
        }

        [Fact]
        public async Task Login_Should_Return_Unauthorized_When_Invalid_Credentials()
        {
            // Arrange
            var loginRequest = new LoginRequest("ademir", "microondas");

            var jsonRequest = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
