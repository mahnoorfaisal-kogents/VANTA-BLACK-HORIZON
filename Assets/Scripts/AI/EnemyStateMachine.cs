namespace Vanta.AI
{
    public enum EnemyState { Idle, Wander, Investigate, Alert, Combat, Chase, Search, Flee, ReturnToNormal, Dead }

    public sealed class EnemyStateMachine
    {
        public EnemyState Current { get; private set; } = EnemyState.Idle;
        public void Transition(EnemyState next) => Current = next;
    }
}
