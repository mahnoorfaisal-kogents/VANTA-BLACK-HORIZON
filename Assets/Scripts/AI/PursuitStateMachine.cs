namespace Vanta.AI
{
    public enum PursuitState { Dormant, Dispatching, Intercepting, Searching, Cooldown }

    public sealed class PursuitStateMachine
    {
        public PursuitState Current { get; private set; } = PursuitState.Dormant;

        public bool Dispatch() => TrySet(PursuitState.Dispatching);
        public bool BeginIntercept() => TrySet(PursuitState.Intercepting);
        public bool LoseTarget() => TrySet(PursuitState.Searching);
        public bool BeginCooldown() => TrySet(PursuitState.Cooldown);
        public bool Reset() => TrySet(PursuitState.Dormant);

        bool TrySet(PursuitState next)
        {
            if (Current == next) return true;
            var allowed = Current switch
            {
                PursuitState.Dormant => next == PursuitState.Dispatching || next == PursuitState.Intercepting,
                PursuitState.Dispatching => next == PursuitState.Intercepting || next == PursuitState.Cooldown || next == PursuitState.Dormant,
                PursuitState.Intercepting => next == PursuitState.Searching || next == PursuitState.Cooldown || next == PursuitState.Dormant,
                PursuitState.Searching => next == PursuitState.Intercepting || next == PursuitState.Cooldown || next == PursuitState.Dormant,
                PursuitState.Cooldown => next == PursuitState.Dormant || next == PursuitState.Dispatching || next == PursuitState.Intercepting,
                _ => false
            };
            if (!allowed) return false;
            Current = next;
            return true;
        }
    }
}
