namespace Vanta.AI
{
    public enum PursuitState
    {
        Dormant,
        Dispatching,
        Intercepting,
        Searching,
        Cooldown
    }

    public sealed class PursuitStateMachine
    {
        public PursuitState Current { get; private set; } = PursuitState.Dormant;

        public void Dispatch() => Current = PursuitState.Dispatching;
        public void BeginIntercept() => Current = PursuitState.Intercepting;
        public void LoseTarget() => Current = PursuitState.Searching;
        public void BeginCooldown() => Current = PursuitState.Cooldown;
        public void Reset() => Current = PursuitState.Dormant;
    }
}
