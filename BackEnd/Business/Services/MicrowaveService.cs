using Business.Exceptions;
using Business.Interfaces;
using System.Timers;

namespace Business.Services
{
    public class MicrowaveService : IMicrowaveService
    {
        private System.Timers.Timer _timer;
        private int _remainingSeconds;
        private int _power;
        private bool _isPaused;
        private readonly object _lock = new();

        // Evento para notificar alterações
        public event Action<string> OnHeatingUpdate;

        public string HeatingDisplay { get; private set; } = "";

        public MicrowaveService()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_isPaused) return;
                if (_remainingSeconds > 0)
                {
                    // Gera a string para este segundo: repete o caractere de aquecimento conforme a potência
                    HeatingDisplay += new string(GetHeatingChar(), _power) + " ";
                    _remainingSeconds--;
                    OnHeatingUpdate?.Invoke(HeatingDisplay);
                }
                else
                {
                    _timer.Stop();
                    HeatingDisplay += "Aquecimento concluído";
                    OnHeatingUpdate?.Invoke(HeatingDisplay);
                }
            }
        }

        private char GetHeatingChar()
        {
            return '.';
        }

        public void StartHeating(int timeSeconds, int? power = null)
        {
            // Valida tempo: mínimo 1 segundo, máximo 2 minutos (120 segundos)
            if (timeSeconds < 1 || timeSeconds > 120)
                throw new MicrowaveException("Tempo inválido. Informe entre 1 segundo e 2 minutos.");

            // Valida potência: se não informado, assume 10; se informado deve estar entre 1 e 10.
            _power = power ?? 10;
            if (_power < 1 || _power > 10)
                throw new MicrowaveException("Potência inválida. Informe um valor entre 1 e 10.");

            _remainingSeconds = timeSeconds;

            if (_timer.Enabled)
            {
                _remainingSeconds += 30;
            }
            else
            {
                HeatingDisplay = string.Empty;
                _isPaused = false;
                _timer.Start();
            }
        }

        public void PauseOrCancel()
        {
            lock (_lock)
            {
                if (_timer.Enabled && !_isPaused)
                {
                    _isPaused = true;
                }
                else if (_isPaused)
                {
                    _timer.Stop();
                    _remainingSeconds = 0;
                    HeatingDisplay = "";
                    _isPaused = false;
                    OnHeatingUpdate?.Invoke(HeatingDisplay);
                }
                else
                {
                    HeatingDisplay = "";
                    OnHeatingUpdate?.Invoke(HeatingDisplay);
                }
            }
        }

        public void ResumeHeating()
        {
            if (_isPaused)
            {
                _isPaused = false;
            }
        }
    }
}

