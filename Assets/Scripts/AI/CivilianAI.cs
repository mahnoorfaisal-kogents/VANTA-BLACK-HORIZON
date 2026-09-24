using UnityEngine;

namespace Vanta.AI
{
    public sealed class CivilianAI : MonoBehaviour
    {
        public enum State { Wander, Investigate, Flee, Recover }
        [SerializeField] private float wanderRadius = 12f, fleeRange = 10f, speed = 1.8f;
        [SerializeField] private Transform threat;
        public State Current { get; private set; } = State.Wander;
        private Vector3 home;

        void Awake() => home = transform.position;

        void Update()
        {
            var distance = threat ? Vector3.Distance(transform.position, threat.position) : float.PositiveInfinity;
            var threatLevel = threat ? Mathf.Clamp01(1f - distance / Mathf.Max(1f, fleeRange * 2f)) : 0f;
            var tactical = AgentTacticalSystem.Evaluate(new AgentTacticalContext(
                threatLevel, threat ? Mathf.Clamp01(1f - threatLevel) : 0.35f, 0.8f,
                threat != null, threat != null && distance <= fleeRange, true));

            if (tactical.Action == AgentAction.Flee)
                Current = State.Flee;
            else if (Current == State.Flee)
                Current = State.Recover;
            else if (threat && distance <= fleeRange * 2f)
                Current = State.Investigate;
            else
                Current = State.Wander;

            if (Current == State.Flee) MoveAway();
            else if (Current == State.Investigate && threat) MoveTo(threat.position);
            else if (Current == State.Wander) Wander();
            else if (Current == State.Recover && Vector3.Distance(transform.position, home) > 1f) MoveTo(home);
        }

        void MoveAway()
        {
            if (!threat) return;
            var d = transform.position - threat.position;
            d.y = 0f;
            if (d.sqrMagnitude > .01f) MoveTo(transform.position + d.normalized * speed);
        }

        void Wander()
        {
            var target = home + new Vector3(Mathf.Sin(Time.time * .23f), 0f, Mathf.Cos(Time.time * .19f)) * wanderRadius;
            MoveTo(target);
        }

        void MoveTo(Vector3 target)
        {
            var d = target - transform.position;
            d.y = 0f;
            if (d.sqrMagnitude < .04f) return;
            transform.position += d.normalized * speed * Time.deltaTime;
            transform.forward = Vector3.Slerp(transform.forward, d.normalized, Time.deltaTime * 5f);
        }

        public void SetThreat(Transform value) => threat = value;
    }
}
