using Moq;
using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Domain.Services;

namespace WebMicroondasTest.Unit.Services
{
    public class MicrowaveServiceTest
    {
        private readonly MicrowaveService _service;
        private readonly Mock<ILogService> _logServiceMock;

        public MicrowaveServiceTest()
        {
            _logServiceMock = new Mock<ILogService>();
            _service = new MicrowaveService(_logServiceMock.Object);
        }

        [Fact]
        public void Start_Should_Set_IsHeating_True_When_Valid_Manual_Parameters()
        {
            // Act
            _service.Start(30, 8, '*', null);
            var status = _service.GetStatus();

            // Assert
            Assert.True(status.IsHeating);
            Assert.Equal(30, status.RemainingTime);
            Assert.Equal(8, status.Power);
            Assert.Equal('*', status.HeatingChar);
            Assert.False(status.IsPaused);
        }

        [Fact]
        public void Start_Should_Use_Program_Parameters_When_Program_Provided()
        {
            // Arrange
            var program = new MicrowaveProgram
            {
                Id = 1,
                Name = "Pipoca",
                TimeSeconds = 180,
                Power = 7,
                HeatingChar = '*',
                IsPredefined = true
            };

            // Act
            _service.Start(30, 5, '.', program);
            var status = _service.GetStatus();

            // Assert
            Assert.True(status.IsHeating);
            Assert.Equal(180, status.RemainingTime);
            Assert.Equal(7, status.Power);
            Assert.Equal('*', status.HeatingChar);
        }

        [Fact]
        public void Start_Should_Throw_Exception_When_Invalid_Time()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _service.Start(0, 8, '*', null)); // Tempo inválido para manual

            Assert.Equal("Informe um tempo entre 1 e 120 segundos.", exception.Message);
        }

        [Fact]
        public void Start_Should_Throw_Exception_When_Invalid_Power()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _service.Start(30, 11, '*', null)); // Potência inválida

            Assert.Equal("Potência deve ser 1..10.", exception.Message);
        }

        [Fact]
        public void Start_Should_Use_Default_Power_When_Null()
        {
            // Act
            _service.Start(30, null, '*', null);
            var status = _service.GetStatus();

            // Assert
            Assert.Equal(10, status.Power); // Deve usar potência padrão 10
        }

        [Fact]
        public void Start_Should_Use_Default_HeatingChar_When_Null()
        {
            // Act
            _service.Start(30, 8, null, null);
            var status = _service.GetStatus();

            // Assert
            Assert.Equal('.', status.HeatingChar); // Deve usar char padrão '.'
        }

        [Fact]
        public void Pause_Should_Set_IsPaused_True()
        {
            // Arrange
            _service.Start(30, 8, '*', null);

            // Act
            _service.Pause();
            var status = _service.GetStatus();

            // Assert
            Assert.True(status.IsPaused);
            Assert.True(status.IsHeating); // Ainda deve estar "heating" mesmo pausado
        }

        [Fact]
        public void Resume_Should_Set_IsPaused_False()
        {
            // Arrange
            _service.Start(30, 8, '*', null);
            _service.Pause();

            // Act
            _service.Resume();
            var status = _service.GetStatus();

            // Assert
            Assert.False(status.IsPaused);
            Assert.True(status.IsHeating);
        }

        [Fact]
        public void Cancel_Should_Stop_Heating()
        {
            // Arrange
            _service.Start(30, 8, '*', null);

            // Act
            _service.Cancel();
            var status = _service.GetStatus();

            // Assert
            Assert.False(status.IsHeating);
            Assert.False(status.IsPaused);
            Assert.Equal(0, status.RemainingTime);
        }

        [Fact]
        public void AddTime_Should_Increase_Remaining_Time_For_Manual_Program()
        {
            // Arrange
            _service.Start(30, 8, '*', null); // Manual
            var initialStatus = _service.GetStatus();

            // Act
            _service.AddTime(30);
            var finalStatus = _service.GetStatus();

            // Assert
            Assert.True(finalStatus.RemainingTime > initialStatus.RemainingTime);
        }

        [Fact]
        public void AddTime_Should_Throw_Exception_For_Predefined_Program()
        {
            // Arrange
            var program = new MicrowaveProgram
            {
                Id = 1,
                TimeSeconds = 180,
                Power = 7,
                HeatingChar = '*',
                IsPredefined = true
            };
            _service.Start(30, 8, '*', program);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _service.AddTime(30));

            Assert.Equal("Não é permitido acrescentar tempo em programas pré-definidos.", exception.Message);
        }

        [Fact]
        public void GetStatus_Should_Return_Correct_Initial_State()
        {
            // Act
            var status = _service.GetStatus();

            // Assert
            Assert.False(status.IsHeating);
            Assert.False(status.IsPaused);
            Assert.Equal(0, status.RemainingTime);
            Assert.Null(status.StartedAt);
        }
    }
}
