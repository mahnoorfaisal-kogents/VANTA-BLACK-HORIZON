using UnityEngine;
using Vanta.Core;

namespace Vanta.AI
{
    [RequireComponent(typeof(Health))]
    public sealed class EnemyAI : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float detectionRange = 18f;
        [SerializeField] private float combatRange = 12f;
        [SerializeField] private float moveSpeed = 3f;
        private Health health;
        public EnemyStateMachine State { get; } = new EnemyStateMachine();

        private void Awake() { health = GetComponent<Health>(); health.Died += Die; }
        private void Update()
        {
            if (!health.IsAlive) return;
            if (!target) { State.Transition(EnemyState.Wander); return; }
            var distance = Vector3.Distance(transform.position, target.position);
            if (distance <= combatRange) State.Transition(EnemyState.Combat);
            else if (distance <= detectionRange) State.Transition(EnemyState.Chase);
            else State.Transition(EnemyState.Wander);
            if (State.Current == EnemyState.Chase)
            {
                var direction = (target.position - transform.position); direction.y = 0f;
                if (direction.sqrMagnitude > .01f) transform.rotation = Quaternion.LookRotation(direction);
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }
        private void Die() => State.Transition(EnemyState.Dead);
    }
}
