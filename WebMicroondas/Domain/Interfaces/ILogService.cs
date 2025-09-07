using WebMicroondas.Models;

namespace WebMicroondas.Domain.Interfaces
{
    public interface ILogService
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message, string exception = null);
        IEnumerable<LogEntry> GetAll();
        void Clear();
    }
}
