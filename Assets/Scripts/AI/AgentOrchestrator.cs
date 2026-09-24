using System;
using UnityEngine;

namespace Vanta.AI
{
    [Serializable]
    public readonly struct AgentContextSnapshot
    {
        public readonly int Tick;
        public readonly float Threat;
        public readonly float Curiosity;
        public readonly float SocialNeed;
        public readonly bool HasTarget;
        public readonly bool PlayerVisible;
        public readonly bool Alive;

        public AgentContextSnapshot(int tick, float threat, float curiosity, float socialNeed,
            bool hasTarget, bool playerVisible, bool alive = true)
        {
            Tick = tick;
            Threat = Mathf.Clamp01(threat);
            Curiosity = Mathf.Clamp01(curiosity);
            SocialNeed = Mathf.Clamp01(socialNeed);
            HasTarget = hasTarget;
            PlayerVisible = playerVisible;
            Alive = alive;
        }
    }

    /// <summary>Reusable perception-to-decision orchestration without requiring an LLM.</summary>
    public sealed class AgentOrchestrator
    {
        public AgentMemoryStore Memory { get; }
        public AgentGoalSystem Goals { get; }
        public AgentActionExecutor Actions { get; }
        public AgentKnowledgeBase Knowledge { get; }

        public AgentOrchestrator(int memoryCapacity = 32)
        {
            Memory = new AgentMemoryStore(memoryCapacity);
            Goals = new AgentGoalSystem();
            Actions = new AgentActionExecutor();
            Knowledge = new AgentKnowledgeBase();
        }

        public AgentAction Tick(AgentContextSnapshot context)
        {
            if (!context.Alive)
            {
                Actions.TryExecute(AgentAction.Idle, false);
                return AgentAction.Idle;
            }

            var decision = AgentDecisionSystem.Choose(new AgentDecisionContext(
                context.Threat, context.Curiosity, context.SocialNeed,
                context.HasTarget, context.PlayerVisible));

            Actions.TryExecute(decision, true);
            Memory.Remember("last_action", decision.ToString(), 0.35f, context.Tick);
            if (context.PlayerVisible)
                Memory.Remember("player_seen", "player visible at tick " + context.Tick, 0.8f, context.Tick);
            return decision;
        }
    }
}