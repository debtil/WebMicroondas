namespace WebMicroondas.Models
{
    public enum MicrowaveLogLevel { Info, Warn, Error }

    /// <summary>
    /// Entrada de log exibida no front.
    /// </summary>
    public sealed record LogEntry(
        DateTime Timestamp,
        MicrowaveLogLevel Level,
        string Message,
        string Exception
    );
}
