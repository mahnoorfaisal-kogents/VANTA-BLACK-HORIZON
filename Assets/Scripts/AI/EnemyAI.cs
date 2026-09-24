using UnityEngine;
using Vanta.Player;

namespace Vanta.AI
{
    [RequireComponent(typeof(Health))]
    public sealed class EnemyAI : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float detectionRange = 18f;
        [SerializeField] private float combatRange = 12f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float attackDamage = 8f;
        [SerializeField] private float attackInterval = 1f;

        private Health health;
        private float nextAttackTime;
        public EnemyStateMachine State { get; } = new EnemyStateMachine();

        private void Awake()
        {
            health = GetComponent<Health>();
            health.Died += Die;
        }

        private void Update()
        {
            if (!health.IsAlive) return;
            if (!target) { State.Transition(EnemyState.Wander); return; }

            var distance = Vector3.Distance(transform.position, target.position);
            if (distance <= combatRange) State.Transition(EnemyState.Combat);
            else if (distance <= detectionRange) State.Transition(EnemyState.Chase);
            else State.Transition(EnemyState.Wander);

            if (State.Current == EnemyState.Chase) MoveTowardTarget();
            else if (State.Current == EnemyState.Combat) AttackTarget();
        }

        private void MoveTowardTarget()
        {
            var direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.01f) return;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        private void AttackTarget()
        {
            var direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(direction);

            if (Time.time < nextAttackTime) return;

            var targetHealth = target.GetComponentInParent<Health>();
            if (targetHealth && targetHealth.IsAlive)
                targetHealth.ApplyDamage(attackDamage, target.position, gameObject);

            nextAttackTime = Time.time + Mathf.Max(0.1f, attackInterval);
        }

        private void Die()
        {
            State.Transition(EnemyState.Dead);
            enabled = false;
        }

        private void OnDestroy()
        {
            if (health) health.Died -= Die;
        }
    }
}
