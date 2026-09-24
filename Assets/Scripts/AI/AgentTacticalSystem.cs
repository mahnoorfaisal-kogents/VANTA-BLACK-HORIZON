using System;
using UnityEngine;

namespace Vanta.AI
{
    public enum AgentTacticalGoal
    {
        Survive,
        Pursue,
        Investigate,
        Assist,
        Patrol
    }

    [Serializable]
    public readonly struct AgentTacticalContext
    {
        public readonly float Threat;
        public readonly float Visibility;
        public readonly float SocialNeed;
        public readonly bool HasTarget;
        public readonly bool TargetVisible;
        public readonly bool Alive;

        public AgentTacticalContext(float threat, float visibility, float socialNeed,
            bool hasTarget, bool targetVisible, bool alive = true)
        {
            Threat = Mathf.Clamp01(threat);
            Visibility = Mathf.Clamp01(visibility);
            SocialNeed = Mathf.Clamp01(socialNeed);
            HasTarget = hasTarget;
            TargetVisible = targetVisible;
            Alive = alive;
        }
    }

    [Serializable]
    public readonly struct AgentTacticalResult
    {
        public readonly AgentTacticalGoal Goal;
        public readonly AgentAction Action;
        public readonly float GoalUtility;

        public AgentTacticalResult(AgentTacticalGoal goal, AgentAction action, float goalUtility)
        {
            Goal = goal;
            Action = action;
            GoalUtility = Mathf.Clamp01(goalUtility);
        }
    }

    /// <summary>
    /// Deterministic tactical arbitration. It converts world context into a stable goal/action pair
    /// so NPCs can adapt without requiring an LLM or network service.
    /// </summary>
    public static class AgentTacticalSystem
    {
        public static AgentTacticalResult Evaluate(AgentTacticalContext context)
        {
            if (!context.Alive)
                return new AgentTacticalResult(AgentTacticalGoal.Survive, AgentAction.Idle, 1f);

            var survive = Mathf.Clamp01(context.Threat * 1.1f);
            var pursue = context.TargetVisible && context.HasTarget ? context.Visibility * 0.8f + context.Threat * 0.2f : 0f;
            var investigate = context.HasTarget && !context.TargetVisible
                ? context.Visibility * 0.55f + (1f - context.Threat) * 0.45f
                : 0f;
            var assist = context.SocialNeed * (1f - context.Threat * 0.5f);
            var patrol = 0.25f + (1f - context.Threat) * 0.15f;

            var goal = AgentTacticalGoal.Patrol;
            var utility = patrol;

            Consider(ref goal, ref utility, AgentTacticalGoal.Assist, assist);
            Consider(ref goal, ref utility, AgentTacticalGoal.Investigate, investigate);
            Consider(ref goal, ref utility, AgentTacticalGoal.Pursue, pursue);
            Consider(ref goal, ref utility, AgentTacticalGoal.Survive, survive);

            var action = goal switch
            {
                AgentTacticalGoal.Survive => AgentAction.Flee,
                AgentTacticalGoal.Pursue => AgentAction.Pursue,
                AgentTacticalGoal.Investigate => AgentAction.Investigate,
                AgentTacticalGoal.Assist => AgentAction.Assist,
                _ => AgentAction.Patrol
            };

            return new AgentTacticalResult(goal, action, utility);
        }

        static void Consider(ref AgentTacticalGoal currentGoal, ref float currentUtility,
            AgentTacticalGoal candidate, float candidateUtility)
        {
            if (candidateUtility > currentUtility ||
                (Mathf.Approximately(candidateUtility, currentUtility) &&
                 (int)candidate < (int)currentGoal))
            {
                currentGoal = candidate;
                currentUtility = candidateUtility;
            }
        }
    }
}
