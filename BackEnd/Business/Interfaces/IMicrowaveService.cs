
namespace Business.Interfaces
{
    public interface IMicrowaveService
    {
        event Action<string> OnHeatingUpdate;
        string HeatingDisplay { get; }
        void StartHeating(int timeSeconds, int? power = null);
        void PauseOrCancel();
        void ResumeHeating();
    }
}
