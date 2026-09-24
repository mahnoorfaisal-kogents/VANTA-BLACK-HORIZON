using System.Collections.Generic;
using UnityEngine;

namespace Vanta.AI
{
    /// <summary>Scene-level AI scheduler. Agents receive a deterministic per-frame execution budget.</summary>
    public sealed class AIFrameBudgetSystem : MonoBehaviour
    {
        [SerializeField, Min(1)] int maxTicksPerFrame = 12;
        AITickBudget budget;
        readonly List<VantaAgentBrain> agents = new();

        public int MaxTicksPerFrame => Mathf.Max(1, maxTicksPerFrame);

        void Awake() => budget = new AITickBudget(MaxTicksPerFrame);

        void Update()
        {
            budget.Reset();
            agents.RemoveAll(a => !a);
        }

        public bool TryAcquire(VantaAgentBrain agent, float priority)
        {
            if (!agent || budget == null) return false;
            if (!agents.Contains(agent)) agents.Add(agent);
            return budget.TryAcquire(agent.GetInstanceID(), priority);
        }
    }
}