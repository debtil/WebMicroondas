using WebMicroondas.Domain.Entities;

namespace WebMicroondas.Domain.Interfaces
{
    public interface IProgramService
    {
        IEnumerable<MicrowaveProgram> GetAll();
        MicrowaveProgram Create(MicrowaveProgram program);
        void Delete(int id);
        bool ExistsHeatingChar(char c);
        MicrowaveProgram GetById(int id);
    }
}
