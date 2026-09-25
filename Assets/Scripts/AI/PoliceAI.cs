using UnityEngine;
using Vanta.Player;
using Vanta.Systems;

namespace Vanta.AI
{
    public sealed class PoliceAI : MonoBehaviour
    {
        [SerializeField] private WantedSystem wanted;
        [SerializeField] private Transform target;
        [SerializeField] private StealthSystem targetStealth;
        [SerializeField] private float detectionRange = 35f;
        [SerializeField] private float interceptRange = 28f;
        [SerializeField] private float searchDuration = 8f;
        [SerializeField] private float cooldownDuration = 10f;
        [SerializeField] private float speed = 5f;

        private readonly PursuitStateMachine pursuit = new();
        private readonly NpcPerceptionModel perception = new();
        private Vector3 lastKnownPosition;
        private float stateTimer;
        private Transform cachedTarget;

        public PursuitState CurrentState => pursuit.Current;

        private void Update()
        {
            if (!wanted || !target)
            {
                pursuit.Reset();
                return;
            }

            RefreshTargetStealth();

            var wantedActive = wanted.Level > 0;
            var distance = Vector3.Distance(transform.position, target.position);
            var suspicion = targetStealth ? targetStealth.Suspicion : (distance <= detectionRange ? 1f : 0f);
            var isHidden = targetStealth && targetStealth.IsHidden;
            var perceptionResult = perception.Evaluate(
                distance,
                detectionRange,
                suspicion,
                isHidden,
                wantedActive);

            var tactical = AgentTacticalSystem.Evaluate(new AgentTacticalContext(
                0.05f,
                perceptionResult.Detected ? 1f : 0.25f,
                0.15f,
                wantedActive,
                perceptionResult.Detected,
                true));

            if (!wantedActive)
            {
                UpdateInvestigation(perceptionResult, distance);
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
                if (perceptionResult.Detected)
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
                if (perceptionResult.Detected && distance <= interceptRange)
                    pursuit.BeginIntercept();
                else if (stateTimer <= 0f)
                {
                    pursuit.BeginCooldown();
                    stateTimer = cooldownDuration;
                }
            }
            else if (pursuit.Current == PursuitState.Cooldown)
            {
                stateTimer -= Time.deltaTime;
                if (perceptionResult.Detected && distance <= interceptRange)
                    pursuit.BeginIntercept();
                else if (stateTimer <= 0f)
                    pursuit.Reset();
            }
        }

        private void UpdateInvestigation(NpcPerceptionResult perceptionResult, float distance)
        {
            if (perceptionResult.Alerted)
            {
                if (pursuit.Current == PursuitState.Dormant || pursuit.Current == PursuitState.Cooldown)
                    pursuit.Dispatch();

                if (pursuit.Current == PursuitState.Dispatching)
                {
                    lastKnownPosition = target.position;
                    pursuit.BeginIntercept();
                }

                if (pursuit.Current == PursuitState.Intercepting)
                    MoveToward(target.position, 1f);

                return;
            }

            if (perceptionResult.Investigating)
            {
                if (pursuit.Current == PursuitState.Dormant || pursuit.Current == PursuitState.Cooldown)
                    pursuit.Dispatch();

                if (pursuit.Current == PursuitState.Dispatching)
                {
                    lastKnownPosition = target.position;
                    stateTimer = searchDuration;
                    pursuit.BeginInvestigation();
                }

                if (pursuit.Current == PursuitState.Investigating)
                {
                    lastKnownPosition = target.position;
                    MoveToward(lastKnownPosition, 0.65f);
                    stateTimer -= Time.deltaTime;

                    if (stateTimer <= 0f)
                    {
                        pursuit.BeginCooldown();
                        stateTimer = cooldownDuration;
                    }
                }

                return;
            }

            if (pursuit.Current == PursuitState.Investigating)
            {
                pursuit.LoseTarget();
                stateTimer = searchDuration;
            }

            if (pursuit.Current == PursuitState.Searching)
            {
                MoveToward(lastKnownPosition, 0.55f);
                stateTimer -= Time.deltaTime;

                if (perceptionResult.Detected)
                    pursuit.BeginIntercept();
                else if (stateTimer <= 0f)
                {
                    pursuit.BeginCooldown();
                    stateTimer = cooldownDuration;
                }
            }
            else if (pursuit.Current == PursuitState.Cooldown)
            {
                stateTimer -= Time.deltaTime;
                if (perceptionResult.Investigating)
                {
                    pursuit.Dispatch();
                    pursuit.BeginInvestigation();
                    stateTimer = searchDuration;
                }
                else if (stateTimer <= 0f)
                    pursuit.Reset();
            }
        }

        private void RefreshTargetStealth()
        {
            if (cachedTarget == target)
                return;

            cachedTarget = target;
            targetStealth = target ? target.GetComponent<StealthSystem>() : null;
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
