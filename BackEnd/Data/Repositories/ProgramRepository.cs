using System.Text.Json;
using Business.Interfaces;
using Business.Models;

namespace Data.Repositories
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly string _filePath = "customPrograms.json";

        public List<CustomProgram> GetCustomPrograms()
        {
            if (!File.Exists(_filePath))
                return new List<CustomProgram>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<CustomProgram>>(json) ?? new List<CustomProgram>();
        }

        public void SaveCustomPrograms(List<CustomProgram> programs)
        {
            var json = JsonSerializer.Serialize(programs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void AddCustomProgram(CustomProgram program)
        {
            var programs = GetCustomPrograms();
            // Validação: o caractere de aquecimento não pode ser '.' nem repetir com programas existentes
            if (program.HeatingChar == '.')
                throw new Exception("Caractere de aquecimento inválido.");

            if (programs.Any(p => p.HeatingChar == program.HeatingChar))
                throw new Exception("Já existe um programa com esse caractere de aquecimento.");

            programs.Add(program);
            SaveCustomPrograms(programs);
        }
    }
}
