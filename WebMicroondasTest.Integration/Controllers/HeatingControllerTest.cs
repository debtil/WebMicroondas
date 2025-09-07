using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;
using WebMicroondas.Models;

namespace WebMicroondasTest.Integration.Controllers
{
    public class HeatingControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HeatingControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthorization(options =>
                    {
                        options.DefaultPolicy = new AuthorizationPolicyBuilder()
                            .RequireAssertion(_ => true)
                            .Build();
                    });
                });
            }).CreateClient();
        }

        [Fact]
        public async Task StartHeating_Should_Return_Success_When_Valid_Request()
        {
            // Arrange
            var startHeatingRequest = new StartHeatingRequest
            {
                Time = 30,
                Power = 8,
                HeatingChar = '*'
            };
            var content = new StringContent(JsonConvert.SerializeObject(startHeatingRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/heating/start", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", responseString);
        }
    }
}
