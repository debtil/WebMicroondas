using Business.Models;

namespace Business.Interfaces
{
    public interface IProgramRepository
    {
        List<CustomProgram> GetCustomPrograms();
        void SaveCustomPrograms(List<CustomProgram> programs);
        void AddCustomProgram(CustomProgram program);
    }
}
