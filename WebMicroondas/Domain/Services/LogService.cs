using System.Collections.Concurrent;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Models;

namespace WebMicroondas.Domain.Services
{
    public class LogService : ILogService
    {
        private readonly ConcurrentQueue<LogEntry> _logs = new();

        private void Push(MicrowaveLogLevel level, string message, string exception = null)
        => _logs.Enqueue(new LogEntry(DateTime.UtcNow, level, message, exception));

        public void Info(string message) => Push(MicrowaveLogLevel.Info, message);
        public void Warn(string message) => Push(MicrowaveLogLevel.Warn, message);
        public void Error(string message, string exception = null) 
            => Push(MicrowaveLogLevel.Error, message, exception);

        public IEnumerable<LogEntry> GetAll() => _logs.ToArray().OrderBy(l => l.Timestamp);
        public void Clear() { while (_logs.TryDequeue(out _)) { } }
    }
}
