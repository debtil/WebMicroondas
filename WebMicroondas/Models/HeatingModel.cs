namespace WebMicroondas.Models
{
    /// <summary>
    /// Requisição para iniciar aquecimento.
    /// Quando ProgramId/ProgramName for informado, tempo/potência/char do programa são aplicados
    /// e valores manuais são ignorados (regras no serviço).
    /// </summary>
    public sealed class StartHeatingRequest
    {
        public int? Time { get; init; }
        public int? Power { get; init; }
        public string? ProgramName { get; init; }
        public char? HeatingChar { get; init; }
        public int? ProgramId { get; init; }
    }

    /// <summary>
    /// Requisição para acrescentar tempo durante aquecimento manual.
    /// </summary>
    public sealed class AddTimeRequest
    {
        public int AdditionalTime { get; init; } = 30;
    }

    /// <summary>
    /// Snapshot do estado do micro-ondas para o front.
    /// </summary>
    public sealed record MicrowaveStatus(
        bool IsHeating,
        bool IsPaused,
        int RemainingTime,
        int Power,
        char HeatingChar,
        DateTime? StartedAt
    );
}
