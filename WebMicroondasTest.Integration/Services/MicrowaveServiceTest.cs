using Moq;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Domain.Services;

namespace WebMicroondasTest.Integration.Services
{
    public class MicrowaveServiceTests
    {
        private readonly MicrowaveService _service;
        private readonly Mock<ILogService> _logServiceMock;

        public MicrowaveServiceTests()
        {
            _logServiceMock = new Mock<ILogService>();
            _service = new MicrowaveService(_logServiceMock.Object);
        }

        [Fact]
        public void Start_Should_Run_Integration()
        {
            _service.Start(10, 5, '*', null);
            var status = _service.GetStatus();
            Assert.True(status.IsHeating);
        }

        [Fact]
        public void Cancel_Should_Work_Integration()
        {
            _service.Cancel();
            var status = _service.GetStatus();
            Assert.False(status.IsHeating);
        }
    }
}
