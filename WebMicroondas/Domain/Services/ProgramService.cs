using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Predefined;

namespace WebMicroondas.Domain.Services
{
    public class ProgramService : IProgramService
    {
        private readonly IProgramRepository _repo;
        public ProgramService(IProgramRepository repo) => _repo = repo;

        public IEnumerable<MicrowaveProgram> GetAll()
        => PredefinedPrograms.Items.Concat(_repo.LoadCustomPrograms());

        public MicrowaveProgram Create(MicrowaveProgram program)
        {
            if (program is null) 
                throw new ArgumentNullException(nameof(program));

            if (string.IsNullOrWhiteSpace(program.Name) || string.IsNullOrWhiteSpace(program.Food))
                throw new InvalidOperationException("Nome e Alimento são obrigatórios.");

            if (program.TimeSeconds < 1) 
                throw new InvalidOperationException("Tempo inválido.");

            if (program.Power is < 1 or > 10) 
                throw new InvalidOperationException("Potência deve ser 1..10.");

            if (program.HeatingChar == '.') 
                throw new InvalidOperationException("Caractere não pode ser '.' (reservado).");

            if (ExistsHeatingChar(program.HeatingChar)) 
                throw new InvalidOperationException("Caractere já utilizado em outro programa.");

            program.Id = _repo.GetNextId();
            program.IsPredefined = false;
            return _repo.Add(program);
        }

        public void Delete(int id)
        {
            if (PredefinedPrograms.Items.Any(p => p.Id == id))
                throw new InvalidOperationException("Programas pré-definidos não podem ser removidos.");
            
            _repo.Delete(id);
        }

        public bool ExistsHeatingChar(char c)
        => PredefinedPrograms.Items.Any(p => p.HeatingChar == c) || _repo.AnyByHeatingChar(c);

        public MicrowaveProgram GetById(int id)
        => GetAll().FirstOrDefault(p => p.Id == id);
    }
}
