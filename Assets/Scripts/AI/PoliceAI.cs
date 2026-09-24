using UnityEngine;
using Vanta.Systems;

namespace Vanta.AI
{
    public sealed class PoliceAI : MonoBehaviour
    {
        [SerializeField] private WantedSystem wanted;
        [SerializeField] private Transform target;
        [SerializeField] private float detectionRange = 35f;
        [SerializeField] private float interceptRange = 28f;
        [SerializeField] private float searchDuration = 8f;
        [SerializeField] private float cooldownDuration = 10f;
        [SerializeField] private float speed = 5f;

        private readonly PursuitStateMachine pursuit = new();
        private Vector3 lastKnownPosition;
        private float stateTimer;

        public PursuitState CurrentState => pursuit.Current;

        private void Update()
        {
            if (!wanted || !target)
            {
                pursuit.Reset();
                return;
            }

            var wantedActive = wanted.Level > 0;
            var distance = Vector3.Distance(transform.position, target.position);
            var targetVisible = distance <= detectionRange;
            var tactical = AgentTacticalSystem.Evaluate(new AgentTacticalContext(
                0.05f, targetVisible ? 1f : 0.25f, 0.15f, wantedActive, targetVisible, true));

            if (!wantedActive)
            {
                if (pursuit.Current != PursuitState.Cooldown && pursuit.Current != PursuitState.Dormant)
                {
                    pursuit.BeginCooldown();
                    stateTimer = cooldownDuration;
                }

                if (pursuit.Current == PursuitState.Cooldown)
                {
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f) pursuit.Reset();
                }
                return;
            }

            if (pursuit.Current == PursuitState.Dormant)
                pursuit.Dispatch();

            if (pursuit.Current == PursuitState.Dispatching)
            {
                lastKnownPosition = target.position;
                pursuit.BeginIntercept();
            }

            if (pursuit.Current == PursuitState.Intercepting)
            {
                if (distance <= detectionRange)
                {
                    lastKnownPosition = target.position;
                    MoveToward(target.position, tactical.Action == AgentAction.Pursue ? 1.15f : 1f);
                }
                else
                {
                    pursuit.LoseTarget();
                    stateTimer = searchDuration;
                }
            }
            else if (pursuit.Current == PursuitState.Searching)
            {
                MoveToward(lastKnownPosition, 0.85f);
                stateTimer -= Time.deltaTime;
                if (distance <= interceptRange)
                {
                    pursuit.BeginIntercept();
                }
                else if (stateTimer <= 0f)
                {
                    pursuit.BeginCooldown();
                    stateTimer = cooldownDuration;
                }
            }
            else if (pursuit.Current == PursuitState.Cooldown)
            {
                stateTimer -= Time.deltaTime;
                if (distance <= interceptRange)
                    pursuit.BeginIntercept();
                else if (stateTimer <= 0f)
                    pursuit.Reset();
            }
        }

        private void MoveToward(Vector3 destination, float speedMultiplier = 1f)
        {
            var direction = destination - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) return;
            transform.forward = Vector3.Slerp(transform.forward, direction.normalized, Time.deltaTime * 5f);
            transform.position += transform.forward * speed * Mathf.Max(0.1f, speedMultiplier) * Time.deltaTime;
        }
    }
}
