using WebMicroondas.Domain.Entities;

namespace WebMicroondas.Domain.Interfaces
{
    public interface IProgramRepository
    {
        IEnumerable<MicrowaveProgram> LoadCustomPrograms();
        MicrowaveProgram Add(MicrowaveProgram program);
        void Delete(int id);
        bool AnyByHeatingChar(char c);
        int GetNextId();
    }
}
