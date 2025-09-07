using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Models;

namespace WebMicroondas.Domain.Services
{
    public class MicrowaveService : IMicrowaveService
    {
        private readonly MicrowaveState _state = new();
        private readonly ILogService _log;

        public MicrowaveService(ILogService log) => _log = log;

        public void Start(int seconds, int? power, char? heatingChar, MicrowaveProgram program)
        {
            try
            {
                // Regras de Nível 1
                var isManual = program is null;

                if (isManual)
                {
                    // 1s..120s
                    if (seconds < 1 || seconds > 120)
                        throw new InvalidOperationException("Informe um tempo entre 1 e 120 segundos.");
                }
                else
                {
                    // Pré-definidos ignoram limite de 120s
                    seconds = program.TimeSeconds;
                    power = program.Power;
                    heatingChar = program.HeatingChar;
                }

                var pwr = (power is null or 0) ? 10 : power.Value;
                if (pwr < 1 || pwr > 10) throw new InvalidOperationException("Potência deve ser 1..10.");

                var ch = heatingChar ?? '.';

                _state.Start(seconds, pwr, ch, program?.Id, program?.IsPredefined ?? false);
                _log.Info($"Início do aquecimento: {seconds}s, potência {pwr}, char '{ch}' {(program != null ? "[programa]" : "[manual]")}");
            }
            catch (Exception ex)
            {
                _log.Error("Falha ao iniciar aquecimento", ex.Message);
                throw;
            }
        }

        public void Pause()
        {
            _state.Pause();
            _log.Info("Aquecimento pausado");
        }

        public void Resume()
        {
            _state.Resume();
            _log.Info("Aquecimento retomado");
        }

        public void Cancel()
        {
            _state.Cancel();
            _log.Warn("Aquecimento cancelado/limpo");
        }

        public void AddTime(int seconds)
        {
            // Nível 1: Só permite acréscimo quando **não** for pré-definido
            var status = GetStatus();
            if (!status.IsHeating || status.IsPaused) return;
            if (IsRunningPredefined()) throw new InvalidOperationException("Não é permitido acrescentar tempo em programas pré-definidos.");
            _state.AddTime(seconds);
            _log.Info($"Tempo acrescido: +{seconds}s");
        }

        public MicrowaveStatus GetStatus()
        {
            var remaining = _state.GetComputedRemainingSeconds();
            var started = _state.StartedAtUtc;
            return new MicrowaveStatus(
            IsHeating: remaining > 0 && (_state.IsHeating || _state.IsPaused),
            IsPaused: _state.IsPaused,
            RemainingTime: remaining,
            Power: _state.Power,
            HeatingChar: _state.HeatingChar,
            StartedAt: started
            );
        }

        private bool IsRunningPredefined() => _state.RunningProgramIsPredefined;
    }
}
