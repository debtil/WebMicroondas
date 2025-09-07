namespace WebMicroondas.Models
{
    /// <summary>
    /// DTO público para listagem/criação de programas.
    /// </summary>
    public sealed record ProgramDto(
        int Id,
        string Name,
        string Food,
        int Time,
        int Power,
        char HeatingChar,
        string Instructions,
        bool IsPredefined
    );
}
