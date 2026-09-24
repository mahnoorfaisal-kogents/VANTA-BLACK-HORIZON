namespace Vanta.AI
{
    public enum EnemyState { Idle, Wander, Investigate, Alert, Combat, Chase, Search, Flee, ReturnToNormal, Dead }

    public sealed class EnemyStateMachine
    {
        public EnemyState Current { get; private set; } = EnemyState.Idle;

        public bool TryTransition(EnemyState next)
        {
            if (Current == next) return true;
            if (Current == EnemyState.Dead) return false;
            if (next == EnemyState.Dead)
            {
                Current = next;
                return true;
            }

            var allowed = Current switch
            {
                EnemyState.Idle => next == EnemyState.Wander || next == EnemyState.Investigate || next == EnemyState.Alert || next == EnemyState.Combat,
                EnemyState.Wander => next == EnemyState.Investigate || next == EnemyState.Alert || next == EnemyState.Combat || next == EnemyState.Idle,
                EnemyState.Investigate => next == EnemyState.Alert || next == EnemyState.Combat || next == EnemyState.Search || next == EnemyState.ReturnToNormal,
                EnemyState.Alert => next == EnemyState.Combat || next == EnemyState.Chase || next == EnemyState.Search || next == EnemyState.ReturnToNormal,
                EnemyState.Combat => next == EnemyState.Chase || next == EnemyState.Search || next == EnemyState.Flee,
                EnemyState.Chase => next == EnemyState.Combat || next == EnemyState.Search || next == EnemyState.Flee,
                EnemyState.Search => next == EnemyState.Alert || next == EnemyState.Combat || next == EnemyState.ReturnToNormal,
                EnemyState.Flee => next == EnemyState.ReturnToNormal || next == EnemyState.Combat,
                EnemyState.ReturnToNormal => next == EnemyState.Idle || next == EnemyState.Wander || next == EnemyState.Alert,
                _ => false
            };
            if (!allowed) return false;
            Current = next;
            return true;
        }

        public bool Transition(EnemyState next) => TryTransition(next);
    }
}