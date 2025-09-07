using WebMicroondas.Domain.Entities;

namespace WebMicroondas.Predefined
{
    public static class PredefinedPrograms
    {
        public static readonly List<MicrowaveProgram> Items = new()
        {
            new() { Id = 1, Name = "Pipoca", Food = "Pipoca (de micro-ondas)", TimeSeconds = 180, Power = 7, HeatingChar = '*', Instructions = "Observar os estouros; se houver > 10s entre eles, interrompa.", IsPredefined = true },
            new() { Id = 2, Name = "Leite", Food = "Leite", TimeSeconds = 300, Power = 5, HeatingChar = '+', Instructions = "Cuidado com líquidos; risco de fervura imediata.", IsPredefined = true },
            new() { Id = 3, Name = "Carnes de boi", Food = "Carne em pedaço/fatias", TimeSeconds = 840, Power = 4, HeatingChar = '#', Instructions = "Na metade, vire o conteúdo para uniformizar.", IsPredefined = true },
            new() { Id = 4, Name = "Frango", Food = "Frango (qualquer corte)", TimeSeconds = 480, Power = 7, HeatingChar = '~', Instructions = "Na metade, vire o conteúdo para uniformizar.", IsPredefined = true },
            new() { Id = 5, Name = "Feijão", Food = "Feijão congelado", TimeSeconds = 480, Power = 9, HeatingChar = '§', Instructions = "Deixe destampado; cuidado ao retirar recipientes plásticos.", IsPredefined = true },
        };
    }
}
