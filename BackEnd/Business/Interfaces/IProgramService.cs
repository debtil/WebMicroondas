using Business.Models;

namespace Business.Interfaces
{
    public interface IProgramService
    {
        List<PredefinedProgram> GetPredefinedPrograms();
    }
}
