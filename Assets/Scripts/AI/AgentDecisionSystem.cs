using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.AI
{
    public enum AgentAction
    {
        Idle,
        Patrol,
        Investigate,
        Flee,
        Pursue,
        Assist,
        Converse
    }

    [Serializable]
    public readonly struct AgentDecisionContext
    {
        public readonly float threat;
        public readonly float curiosity;
        public readonly float socialNeed;
        public readonly bool hasTarget;
        public readonly bool playerVisible;

        public AgentDecisionContext(float threat, float curiosity, float socialNeed, bool hasTarget, bool playerVisible)
        {
            this.threat = Mathf.Clamp01(threat);
            this.curiosity = Mathf.Clamp01(curiosity);
            this.socialNeed = Mathf.Clamp01(socialNeed);
            this.hasTarget = hasTarget;
            this.playerVisible = playerVisible;
        }
    }

    /// <summary>Small utility scorer for predictable NPC behavior; ties are resolved deterministically.</summary>
    public static class AgentDecisionSystem
    {
        public static AgentAction Choose(AgentDecisionContext context)
        {
            var scores = new Dictionary<AgentAction, float>
            {
                [AgentAction.Flee] = context.threat * 1.2f,
                [AgentAction.Pursue] = context.playerVisible && context.hasTarget ? context.threat : 0f,
                [AgentAction.Investigate] = context.curiosity * (context.hasTarget ? 1f : 0.25f),
                [AgentAction.Assist] = context.socialNeed * (context.hasTarget ? 1f : 0.2f),
                [AgentAction.Converse] = context.socialNeed * 0.7f,
                [AgentAction.Patrol] = 0.25f,
                [AgentAction.Idle] = 0.1f
            };

            var best = AgentAction.Idle;
            var bestScore = float.MinValue;
            foreach (var action in scores)
            {
                if (action.Value > bestScore)
                {
                    bestScore = action.Value;
                    best = action.Key;
                }
            }
            return best;
        }
    }
}
