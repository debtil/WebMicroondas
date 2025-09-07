using Moq;
using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Domain.Services;

namespace WebMicroondasTests.Services
{
    public class MicrowaveServiceTests
    {
        private readonly MicrowaveService _microwaveService;
        private readonly Mock<ILogService> _logServiceMock;

        public MicrowaveServiceTests()
        {
            _logServiceMock = new Mock<ILogService>();
            _microwaveService = new MicrowaveService(_logServiceMock.Object);
        }

        [Fact]
        public void Start_Should_Start_Heating_When_Valid_Manual_Parameters()
        {
            // Arrange
            int time = 30;
            int power = 8;
            char heatingChar = '*';
            MicrowaveProgram program = null; // Simula-se que não há programa

            // Act
            _microwaveService.Start(time, power, heatingChar, program);

            // Assert
            // Verifica se o log de aquecimento foi gerado corretamente
            _logServiceMock.Verify(log => log.Info(It.Is<string>(s => s.Contains("Início do aquecimento"))), Times.Once);
        }

        [Fact]
        public void Start_Should_Start_Heating_When_Program_Is_Provided()
        {
            // Arrange
            var program = new MicrowaveProgram
            {
                Name = "Pipoca",
                Food = "Pipoca de micro-ondas",
                TimeSeconds = 180,
                Power = 7,
                HeatingChar = '*',
                Instructions = "Aquecer pipoca",
                IsPredefined = true
            };
            int time = 0; // O tempo será ignorado porque o programa será usado
            int power = 0; // A potência será definida pelo programa
            char heatingChar = '*'; // O caractere de aquecimento também será determinado pelo programa

            // Act
            _microwaveService.Start(time, power, heatingChar, program);

            // Assert
            _logServiceMock.Verify(log => log.Info(It.Is<string>(s => s.Contains("Início do aquecimento"))), Times.Once);
        }

        [Fact]
        public void Start_Should_Throw_Exception_When_Invalid_Time()
        {
            // Arrange
            int invalidTime = 0; // Tempo inválido
            int power = 8;
            char heatingChar = '*';
            MicrowaveProgram program = null; // Sem programa

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _microwaveService.Start(invalidTime, power, heatingChar, program));

            Assert.Equal("Informe um tempo entre 1 e 120 segundos.", exception.Message);
        }

        [Fact]
        public void Start_Should_Throw_Exception_When_Invalid_Power()
        {
            // Arrange
            int time = 30;
            int invalidPower = 11; // Potência inválida (maior que 10)
            char heatingChar = '*';
            MicrowaveProgram program = null; // Sem programa

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _microwaveService.Start(time, invalidPower, heatingChar, program));

            Assert.Equal("Potência deve ser entre 1 e 10.", exception.Message);
        }
    }
}
