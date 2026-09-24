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
        int budgetFrame = -1;

        public int MaxTicksPerFrame => Mathf.Max(1, maxTicksPerFrame);

        void Awake()
        {
            budget = new AITickBudget(MaxTicksPerFrame);
            budgetFrame = Time.frameCount;
        }

        void Update()
        {
            ResetForCurrentFrame();
            agents.RemoveAll(a => !a);
        }

        public bool TryAcquire(VantaAgentBrain agent, float priority)
        {
            if (!agent) return false;
            if (budget == null) budget = new AITickBudget(MaxTicksPerFrame);
            ResetForCurrentFrame();
            if (!agents.Contains(agent)) agents.Add(agent);
            return budget.TryAcquire(agent.GetInstanceID(), priority);
        }

        void ResetForCurrentFrame()
        {
            if (budgetFrame == Time.frameCount) return;
            budget.Reset();
            budgetFrame = Time.frameCount;
        }
    }
}
