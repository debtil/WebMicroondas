using Newtonsoft.Json;
using System.Text;
using WebMicroondas.Models;
using WebMicroondasTests.Helpers;

namespace WebMicroondasTests.Controllers
{
    public class HeatingControllerTests
    {
        private readonly HttpClient _client;

        public HeatingControllerTests()
        {
            _client = ServerHelperTests.GetTestServerClient().Result;
        }

        [Fact]
        public async Task StartHeating_Should_Return_Status_When_Valid_Request()
        {
            // Arrange
            var startHeatingRequest = new StartHeatingRequest
            {
                Time = 30,
                Power = 8,
                HeatingChar = '*'
            };

            var jsonRequest = JsonConvert.SerializeObject(startHeatingRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/heating/start", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se a resposta foi sucesso
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", responseString); // Verifica se o sucesso está na resposta
        }

        [Fact]
        public async Task StartHeating_Should_Return_BadRequest_When_Invalid_Request()
        {
            // Arrange
            var startHeatingRequest = new StartHeatingRequest
            {
                Time = -1, // Tempo inválido
                Power = 15, // Potência inválida
                HeatingChar = '*'
            };

            var jsonRequest = JsonConvert.SerializeObject(startHeatingRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/heating/start", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode); // Deve retornar BadRequest
        }

        [Fact]
        public async Task PauseHeating_Should_Return_Status()
        {
            // Arrange
            var response = await _client.PostAsync("/api/heating/pause", null);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se a resposta foi sucesso
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", responseString); // Verifica se o sucesso está na resposta
        }

        [Fact]
        public async Task ResumeHeating_Should_Return_Status()
        {
            // Arrange
            var response = await _client.PostAsync("/api/heating/resume", null);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se a resposta foi sucesso
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", responseString); // Verifica se o sucesso está na resposta
        }

        [Fact]
        public async Task CancelHeating_Should_Return_Status()
        {
            // Arrange
            var response = await _client.PostAsync("/api/heating/cancel", null);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se a resposta foi sucesso
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", responseString); // Verifica se o sucesso está na resposta
        }
    }
}
