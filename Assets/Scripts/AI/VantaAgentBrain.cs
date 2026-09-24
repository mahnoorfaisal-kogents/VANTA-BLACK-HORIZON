using UnityEngine;

namespace Vanta.AI
{
    /// <summary>
    /// Lightweight MonoBehaviour adapter for the deterministic agent stack.
    /// Existing EnemyAI/PoliceAI systems remain authoritative for movement/combat.
    /// </summary>
    public sealed class VantaAgentBrain : MonoBehaviour
    {
        [SerializeField, Min(1)] int memoryCapacity = 32;
        [SerializeField, Min(1)] int ticksPerSecond = 5;
        [SerializeField, Range(0f, 1f)] float curiosity = 0.35f;
        [SerializeField, Range(0f, 1f)] float socialNeed = 0.2f;
        [SerializeField] bool enabledBrain = true;

        AgentOrchestrator orchestrator;
        float accumulator;
        int tick;

        public AgentAction LastAction => orchestrator?.Actions.LastAction ?? AgentAction.Idle;
        public AgentMemoryStore Memory => orchestrator?.Memory;
        public AgentKnowledgeBase Knowledge => orchestrator?.Knowledge;

        void Awake() => orchestrator = new AgentOrchestrator(memoryCapacity);

        void Update()
        {
            if (!enabledBrain || ticksPerSecond <= 0) return;
            accumulator += Time.deltaTime;
            var interval = 1f / ticksPerSecond;
            if (accumulator < interval) return;
            accumulator -= interval;
            Evaluate();
        }

        void Evaluate()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var hasTarget = player != null;
            var playerVisible = false;
            var threat = 0f;

            if (player)
            {
                var distance = Vector3.Distance(transform.position, player.transform.position);
                playerVisible = distance <= 30f;
                threat = Mathf.Clamp01(1f - distance / 30f);
            }

            var context = new AgentContextSnapshot(
                ++tick, threat, curiosity, socialNeed,
                hasTarget, playerVisible, true);

            orchestrator.Tick(context);
        }
    }
}