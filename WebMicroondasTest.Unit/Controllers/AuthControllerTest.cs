using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using WebMicroondas.Controllers;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Infra.Security;
using WebMicroondas.Models;

namespace WebMicroondasTest.Unit.Controllers
{
    public class AuthControllerTest
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly TokenService _tokenService;
        private readonly Mock<ITokenService> _tokenInterfaceMock;
        private readonly AuthController _controller;

        public AuthControllerTest()
        {
            _configurationMock = new Mock<IConfiguration>();

            var configForTokenService = new Mock<IConfiguration>();
            configForTokenService.Setup(c => c["Jwt:Key"]).Returns("test-key-with-minimum-32-characters-length");
            configForTokenService.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            configForTokenService.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            _tokenService = new TokenService(configForTokenService.Object);

            _tokenInterfaceMock = new Mock<ITokenService>();

            _controller = new AuthController(_configurationMock.Object, _tokenService, _tokenInterfaceMock.Object);
        }

        [Fact]
        public void Login_Should_Return_Token_When_Valid_Credentials()
        {
            // Arrange
            var loginRequest = new LoginRequest("admin", "microwave123");
            _configurationMock.Setup(c => c["Auth:Username"]).Returns("admin");
            _configurationMock.Setup(c => c["Auth:PasswordHash"]).Returns(PasswordHasher.Sha256("microwave123"));

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var responseJson = JsonConvert.SerializeObject(okResult.Value);
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.True((bool)response.success);
            Assert.NotNull(response.data.Token);
        }

        [Fact]
        public void Login_Should_Return_Unauthorized_When_Invalid_Credentials()
        {
            // Arrange
            var loginRequest = new LoginRequest("invalid", "invalid");
            _configurationMock.Setup(c => c["Auth:Username"]).Returns("admin");
            _configurationMock.Setup(c => c["Auth:PasswordHash"]).Returns(PasswordHasher.Sha256("microwave123"));

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);

            var responseJson = JsonConvert.SerializeObject(unauthorizedResult.Value);
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.False((bool)response.success);
            Assert.Equal("Credenciais inválidas", (string)response.message);
        }
    }
}
