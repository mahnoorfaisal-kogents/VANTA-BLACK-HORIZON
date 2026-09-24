using UnityEngine;

namespace Vanta.AI
{
    /// <summary>Lightweight adapter for deterministic agent intelligence. Specialist controllers remain authoritative for movement/combat.</summary>
    public sealed class VantaAgentBrain : MonoBehaviour
    {
        [SerializeField, Min(1)] int memoryCapacity = 32;
        [SerializeField, Min(1)] int ticksPerSecond = 5;
        [SerializeField, Range(0f, 1f)] float curiosity = 0.35f;
        [SerializeField, Range(0f, 1f)] float socialNeed = 0.2f;
        [SerializeField, Range(1f, 100f)] float perceptionRange = 30f;
        [SerializeField] bool enabledBrain = true;

        AgentOrchestrator orchestrator;
        AgentTelemetry telemetry;
        AIFrameBudgetSystem frameBudget;
        Transform player;
        float accumulator;
        int tick;

        public AgentAction LastAction => orchestrator?.Actions.LastAction ?? AgentAction.Idle;
        public AgentMemoryStore Memory => orchestrator?.Memory;
        public AgentKnowledgeBase Knowledge => orchestrator?.Knowledge;
        public AgentTelemetry Telemetry => telemetry;

        void Awake()
        {
            orchestrator = new AgentOrchestrator(memoryCapacity);
            telemetry = new AgentTelemetry();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            frameBudget = FindObjectOfType<AIFrameBudgetSystem>();
        }

        void Update()
        {
            if (!enabledBrain || ticksPerSecond <= 0) return;
            accumulator += Time.deltaTime;
            var interval = 1f / ticksPerSecond;
            if (accumulator < interval) return;
            accumulator %= interval;

            if (frameBudget && !frameBudget.TryAcquire(this, 0.5f)) return;
            Evaluate();
        }

        void Evaluate()
        {
            if (!orchestrator) return;
            if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;

            var hasTarget = player != null;
            var playerVisible = false;
            var threat = 0f;

            if (player)
            {
                var distance = Vector3.Distance(transform.position, player.position);
                playerVisible = distance <= perceptionRange;
                threat = Mathf.Clamp01(1f - distance / perceptionRange);
            }

            var context = new AgentContextSnapshot(++tick, threat, curiosity, socialNeed,
                hasTarget, playerVisible, true);
            var action = orchestrator.Tick(context);
            if (playerVisible && player)
                orchestrator.Knowledge.AddFact("last_known_player", player.position.ToString(), 0.9f);
            else if (hasTarget)
                orchestrator.Knowledge.AddFact("player_lost", "target not currently visible", 0.7f);
            telemetry.Record(new AgentDecisionTrace(tick, action, threat));
        }
    }
}