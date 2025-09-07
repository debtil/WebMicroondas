using Microsoft.AspNetCore.Mvc;
using Moq;
using WebMicroondas.Controllers;
using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Models;

namespace WebMicroondasTest.Unit.Controllers
{
    public class HeatingControllerTest
    {
        private readonly Mock<IMicrowaveService> _serviceMock;
        private readonly Mock<IProgramService> _programServiceMock;
        private readonly HeatingController _controller;

        public HeatingControllerTest()
        {
            _serviceMock = new Mock<IMicrowaveService>();
            _programServiceMock = new Mock<IProgramService>();
            _controller = new HeatingController(_serviceMock.Object, _programServiceMock.Object);
        }

        [Fact]
        public void Start_Should_Return_Success_With_Manual_Parameters()
        {
            // Arrange
            var request = new StartHeatingRequest { Time = 30, Power = 8, HeatingChar = '*' };
            var expectedStatus = new MicrowaveStatus(
                IsHeating: true,
                IsPaused: false,
                RemainingTime: 30,
                Power: 8,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow
            );

            _serviceMock.Setup(s => s.Start(30, 8, '*', null)).Verifiable();
            _serviceMock.Setup(s => s.GetStatus()).Returns(expectedStatus);

            // Act
            var result = _controller.Start(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            _serviceMock.Verify(s => s.Start(30, 8, '*', null), Times.Once);
            _serviceMock.Verify(s => s.GetStatus(), Times.Once);
        }

        [Fact]
        public void Start_Should_Return_Success_With_Program_By_Id()
        {
            // Arrange
            var program = new MicrowaveProgram
            {
                Id = 1,
                Name = "Pipoca",
                TimeSeconds = 180,
                Power = 7,
                HeatingChar = '*'
            };
            var request = new StartHeatingRequest { ProgramId = 1, HeatingChar = '*' };
            var expectedStatus = new MicrowaveStatus(
                IsHeating: true,
                IsPaused: false,
                RemainingTime: 180,
                Power: 7,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow
            );

            _programServiceMock.Setup(p => p.GetById(1)).Returns(program);
            _serviceMock.Setup(s => s.Start(180, 7, '*', program)).Verifiable();
            _serviceMock.Setup(s => s.GetStatus()).Returns(expectedStatus);

            // Act
            var result = _controller.Start(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _programServiceMock.Verify(p => p.GetById(1), Times.Once);
            _serviceMock.Verify(s => s.Start(180, 7, '*', program), Times.Once);
        }

        [Fact]
        public void Start_Should_Return_Success_With_Program_By_Name()
        {
            // Arrange
            var program = new MicrowaveProgram
            {
                Id = 1,
                Name = "Pipoca",
                TimeSeconds = 180,
                Power = 7,
                HeatingChar = '*'
            };
            var programs = new[] { program };
            var request = new StartHeatingRequest { ProgramName = "Pipoca", HeatingChar = '*' };
            var expectedStatus = new MicrowaveStatus(
                IsHeating: true,
                IsPaused: false,
                RemainingTime: 180,
                Power: 7,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow
            );

            _programServiceMock.Setup(p => p.GetAll()).Returns(programs);
            _serviceMock.Setup(s => s.Start(180, 7, '*', program)).Verifiable();
            _serviceMock.Setup(s => s.GetStatus()).Returns(expectedStatus);

            // Act
            var result = _controller.Start(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _programServiceMock.Verify(p => p.GetAll(), Times.Once);
            _serviceMock.Verify(s => s.Start(180, 7, '*', program), Times.Once);
        }

        [Fact]
        public void Start_Should_Return_BadRequest_When_Exception_Occurs()
        {
            // Arrange
            var request = new StartHeatingRequest { Time = 0, Power = 8, HeatingChar = '*' };

            _serviceMock.Setup(s => s.Start(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<char>(), It.IsAny<MicrowaveProgram>()))
                       .Throws(new InvalidOperationException("Invalid time"));

            // Act
            var result = _controller.Start(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public void Status_Should_Return_Current_Status()
        {
            // Arrange
            var expectedStatus = new MicrowaveStatus(
                IsHeating: false,
                IsPaused: false,
                RemainingTime: 0,
                Power: 0,
                HeatingChar: '.',
                StartedAt: null
            );
            _serviceMock.Setup(s => s.GetStatus()).Returns(expectedStatus);

            // Act
            var result = _controller.Status();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _serviceMock.Verify(s => s.GetStatus(), Times.Once);
        }

        [Fact]
        public void Pause_Should_Call_Service_Pause()
        {
            // Arrange
            var expectedStatus = new MicrowaveStatus(
                IsHeating: false,
                IsPaused: true,
                RemainingTime: 60,
                Power: 8,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow
            );
            _serviceMock.Setup(s => s.GetStatus()).Returns(expectedStatus);

            // Act
            var result = _controller.Pause();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _serviceMock.Verify(s => s.Pause(), Times.Once);
            _serviceMock.Verify(s => s.GetStatus(), Times.Once);
        }

        [Fact]
        public void Resume_Should_Call_Service_Resume()
        {
            // Arrange
            var expectedStatus = new MicrowaveStatus(
                IsHeating: true,
                IsPaused: false,
                RemainingTime: 60,
                Power: 8,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow
            );
            _serviceMock.Setup(s => s.GetStatus()).Returns(expectedStatus);

            // Act
            var result = _controller.Resume();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _serviceMock.Verify(s => s.Resume(), Times.Once);
            _serviceMock.Verify(s => s.GetStatus(), Times.Once);
        }

        [Fact]
        public void Cancel_Should_Call_Service_Cancel()
        {
            // Act
            var result = _controller.Cancel();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _serviceMock.Verify(s => s.Cancel(), Times.Once);
        }

        [Fact]
        public void AddTime_Should_Add_Default_30_Seconds_When_No_Time_Provided()
        {
            // Arrange
            var request = new AddTimeRequest(); // AdditionalTime já tem valor padrão 30
            var expectedStatus = new MicrowaveStatus(
                IsHeating: true,
                IsPaused: false,
                RemainingTime: 90, // 60 + 30 segundos adicionados
                Power: 8,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow
            );
            _serviceMock.Setup(s => s.GetStatus()).Returns(expectedStatus);

            // Act
            var result = _controller.AddTime(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _serviceMock.Verify(s => s.AddTime(30), Times.Once); // Deve usar 30 como padrão
        }
    }
}
