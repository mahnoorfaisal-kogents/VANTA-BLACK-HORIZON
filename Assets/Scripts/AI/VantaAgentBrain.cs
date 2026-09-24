using UnityEngine;

namespace Vanta.AI
{
    /// <summary>Lightweight adapter for deterministic agent intelligence. Movement/combat remain owned by specialist controllers.</summary>
    public sealed class VantaAgentBrain : MonoBehaviour
    {
        [SerializeField, Min(1)] int memoryCapacity = 32;
        [SerializeField, Min(1)] int ticksPerSecond = 5;
        [SerializeField, Range(0f, 1f)] float curiosity = 0.35f;
        [SerializeField, Range(0f, 1f)] float socialNeed = 0.2f;
        [SerializeField, Range(1f, 100f)] float perceptionRange = 30f;
        [SerializeField] bool enabledBrain = true;

        AgentOrchestrator orchestrator;
        Transform player;
        float accumulator;
        int tick;

        public AgentAction LastAction => orchestrator?.Actions.LastAction ?? AgentAction.Idle;
        public AgentMemoryStore Memory => orchestrator?.Memory;
        public AgentKnowledgeBase Knowledge => orchestrator?.Knowledge;

        void Awake()
        {
            orchestrator = new AgentOrchestrator(memoryCapacity);
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        void Update()
        {
            if (!enabledBrain || ticksPerSecond <= 0) return;
            if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
            accumulator += Time.deltaTime;
            var interval = 1f / ticksPerSecond;
            if (accumulator < interval) return;
            accumulator %= interval;
            Evaluate();
        }

        void Evaluate()
        {
            if (!orchestrator) return;
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
            orchestrator.Tick(context);
        }
    }
}