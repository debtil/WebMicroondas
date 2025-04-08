using Business.Interfaces;
using Business.Models;

namespace Business.Services
{
    public class ProgramService : IProgramService
    {
        private readonly List<PredefinedProgram> _predefinedPrograms;

        public ProgramService()
        {
            // Inicializa os programas pré-definidos conforme especificado
            _predefinedPrograms = new List<PredefinedProgram>
            {
                new PredefinedProgram
                {
                    Name = "Pipoca",
                    Food = "Pipoca (de micro-ondas)",
                    TimeSeconds = 180,
                    Power = 7,
                    HeatingChar = '*',
                    Instructions = "Observar o barulho de estouros do milho..."
                },
                new PredefinedProgram
                {
                    Name = "Leite",
                    Food = "Leite",
                    TimeSeconds = 300,
                    Power = 5,
                    HeatingChar = '#',
                    Instructions = "Cuidado com aquecimento de líquidos..."
                },
                new PredefinedProgram
                {
                    Name = "Carnes de boi",
                    Food = "Carne em pedaço ou fatias",
                    TimeSeconds = 840,
                    Power = 4,
                    HeatingChar = '@',
                    Instructions = "Interrompa o processo na metade e vire o conteúdo..."
                },
                new PredefinedProgram
                {
                    Name = "Frango",
                    Food = "Frango (qualquer corte)",
                    TimeSeconds = 480,
                    Power = 7,
                    HeatingChar = '$',
                    Instructions = "Interrompa o processo na metade e vire o conteúdo..."
                },
                new PredefinedProgram
                {
                    Name = "Feijão",
                    Food = "Feijão congelado",
                    TimeSeconds = 480,
                    Power = 9,
                    HeatingChar = '%',
                    Instructions = "Deixe o recipiente destampado..."
                },
            };
        }

        public List<PredefinedProgram> GetPredefinedPrograms() => _predefinedPrograms;
    }
}
