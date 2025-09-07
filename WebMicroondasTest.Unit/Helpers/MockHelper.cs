using Moq;
using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Models;

namespace WebMicroondasTest.Unit.Helpers
{
    public static class MockHelper
    {
        public static Mock<IMicrowaveService> GetMicrowaveServiceMock()
        {
            var mock = new Mock<IMicrowaveService>();

            mock.Setup(s => s.Start(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<char?>(), It.IsAny<MicrowaveProgram>()));

            mock.Setup(s => s.GetStatus()).Returns(new MicrowaveStatus(
                IsHeating: false,
                IsPaused: false,
                RemainingTime: 0,
                Power: 0,
                HeatingChar: '.',
                StartedAt: null
            ));

            return mock;
        }

        public static Mock<IMicrowaveService> GetMicrowaveServiceMockWithRunningStatus()
        {
            var mock = new Mock<IMicrowaveService>();

            mock.Setup(s => s.Start(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<char?>(), It.IsAny<MicrowaveProgram>()));

            mock.Setup(s => s.GetStatus()).Returns(new MicrowaveStatus(
                IsHeating: true,
                IsPaused: false,
                RemainingTime: 30,
                Power: 8,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow
            ));

            return mock;
        }

        public static Mock<IMicrowaveService> GetMicrowaveServiceMockWithPausedStatus()
        {
            var mock = new Mock<IMicrowaveService>();

            mock.Setup(s => s.Start(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<char?>(), It.IsAny<MicrowaveProgram>()));

            mock.Setup(s => s.GetStatus()).Returns(new MicrowaveStatus(
                IsHeating: true,
                IsPaused: true,
                RemainingTime: 15,
                Power: 5,
                HeatingChar: '*',
                StartedAt: DateTime.UtcNow.AddMinutes(-1)
            ));

            return mock;
        }

        public static Mock<ILogService> GetLogServiceMock()
        {
            return new Mock<ILogService>();
        }

        public static Mock<IProgramService> GetProgramServiceMock()
        {
            var mock = new Mock<IProgramService>();

            mock.Setup(p => p.GetAll()).Returns(new List<MicrowaveProgram>());
            mock.Setup(p => p.GetById(It.IsAny<int>())).Returns((MicrowaveProgram)null);

            return mock;
        }

        public static Mock<IProgramService> GetProgramServiceMockWithPrograms()
        {
            var mock = new Mock<IProgramService>();

            var programs = new List<MicrowaveProgram>
            {
                new MicrowaveProgram
                {
                    Id = 1,
                    Name = "Pipoca",
                    Food = "Pipoca de micro-ondas",
                    TimeSeconds = 180,
                    Power = 7,
                    HeatingChar = '*',
                    Instructions = "Aquecer pipoca",
                    IsPredefined = true
                },
                new MicrowaveProgram
                {
                    Id = 2,
                    Name = "Pizza",
                    Food = "Pizza congelada",
                    TimeSeconds = 120,
                    Power = 8,
                    HeatingChar = '#',
                    Instructions = "Aquecer pizza",
                    IsPredefined = true
                }
            };

            mock.Setup(p => p.GetAll()).Returns(programs);
            mock.Setup(p => p.GetById(1)).Returns(programs[0]);
            mock.Setup(p => p.GetById(2)).Returns(programs[1]);
            mock.Setup(p => p.GetById(It.Is<int>(id => id != 1 && id != 2))).Returns((MicrowaveProgram)null);

            return mock;
        }
    }
}
