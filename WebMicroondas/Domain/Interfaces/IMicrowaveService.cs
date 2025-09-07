using WebMicroondas.Domain.Entities;
using WebMicroondas.Models;

namespace WebMicroondas.Domain.Interfaces
{
    public interface IMicrowaveService
    {
        void Start(int seconds, int? power, char? heatingChar, MicrowaveProgram programOrNull);
        void Pause();
        void Resume();
        void Cancel();
        void AddTime(int seconds);
        MicrowaveStatus GetStatus();
    }
}
