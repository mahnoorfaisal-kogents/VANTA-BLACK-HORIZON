using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.AI
{
    [Serializable]
    public readonly struct AgentGoal
    {
        public readonly string Id;
        public readonly float Utility;
        public readonly bool Enabled;
        public AgentGoal(string id, float utility, bool enabled = true)
        {
            Id = id ?? string.Empty;
            Utility = Mathf.Clamp01(utility);
            Enabled = enabled;
        }
    }

    /// <summary>Deterministic goal arbitration layer above perception and below action selection.</summary>
    public sealed class AgentGoalSystem
    {
        readonly Dictionary<string, AgentGoal> goals = new();

        public void AddOrUpdate(AgentGoal goal)
        {
            if (!string.IsNullOrWhiteSpace(goal.Id)) goals[goal.Id] = goal;
        }

        public bool Remove(string id) => goals.Remove(id);

        public AgentGoal SelectGoal()
        {
            var selected = default(AgentGoal);
            var found = false;
            foreach (var goal in goals.Values)
            {
                if (!goal.Enabled) continue;
                if (!found || goal.Utility > selected.Utility ||
                    (Mathf.Approximately(goal.Utility, selected.Utility) &&
                     string.CompareOrdinal(goal.Id, selected.Id) < 0))
                {
                    selected = goal;
                    found = true;
                }
            }
            return selected;
        }
    }
}