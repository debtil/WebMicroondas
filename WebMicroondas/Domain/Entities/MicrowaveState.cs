namespace WebMicroondas.Domain.Entities
{
        public class MicrowaveState
        {
            private readonly object _gate = new();

            public bool IsHeating { get; private set; }
            public bool IsPaused { get; private set; }
            public int Power { get; private set; } = 10;
            public int RemainingSeconds { get; private set; }
            public DateTime? StartedAtUtc { get; private set; }
            public char HeatingChar { get; private set; } = '.';
            public int? RunningProgramId { get; private set; }
            public bool RunningProgramIsPredefined { get; private set; }

            // INICIAR corretamente (ativar aquecimento)
            public void Start(int seconds, int power, char heatingChar, int? programId, bool isPredefined)
            {
                lock (_gate)
                {
                    IsHeating = true;
                    IsPaused = false;
                    RemainingSeconds = seconds;
                    Power = power;
                    HeatingChar = heatingChar;
                    StartedAtUtc = DateTime.UtcNow;
                    RunningProgramId = programId;
                    RunningProgramIsPredefined = isPredefined;
                }
            }

            // PAUSAR
            public void Pause()
            {
                lock (_gate)
                {
                    if (!IsHeating || IsPaused) return;
                    RemainingSeconds = GetComputedRemainingSeconds_NoLock();
                    IsPaused = true;
                    StartedAtUtc = null;
                }
            }

            // RETOMAR
            public void Resume()
            {
                lock (_gate)
                {
                    if (!IsHeating || !IsPaused) return;
                    IsPaused = false;
                    StartedAtUtc = DateTime.UtcNow;
                }
            }

            // CANCELAR / LIMPAR
            public void Cancel()
            {
                lock (_gate)
                {
                    IsHeating = false;
                    IsPaused = false;
                    RemainingSeconds = 0;
                    Power = 10;
                    HeatingChar = '.';
                    StartedAtUtc = null;
                    RunningProgramId = null;
                    RunningProgramIsPredefined = false;
                }
            }

            // ACRÉSCIMO DE TEMPO (somente se aquecendo e não pausado)
            public void AddTime(int seconds)
            {
                lock (_gate)
                {
                    if (!IsHeating || IsPaused) return;
                    var curr = GetComputedRemainingSeconds_NoLock();
                    RemainingSeconds = curr + seconds;
                    StartedAtUtc = DateTime.UtcNow;
                }
            }

            public int GetComputedRemainingSeconds()
            {
                lock (_gate)
                {
                    return GetComputedRemainingSeconds_NoLock();
                }
            }

            private int GetComputedRemainingSeconds_NoLock()
            {
                if (!IsHeating) return 0;
                if (IsPaused || StartedAtUtc is null) return RemainingSeconds;

                var elapsed = (int)Math.Floor((DateTime.UtcNow - StartedAtUtc.Value).TotalSeconds);
                var left = Math.Max(0, RemainingSeconds - elapsed);

                if (left == 0)
                {
                    // auto-finaliza
                    IsHeating = false;
                    IsPaused = false;
                    StartedAtUtc = null;
                }

                return left;
            }

            public int GetElapsedSeconds()
            {
                lock (_gate)
                {
                    if (!IsHeating || StartedAtUtc is null) return 0;
                    return (int)Math.Floor((DateTime.UtcNow - StartedAtUtc.Value).TotalSeconds);
                }
            }
        }
    }

