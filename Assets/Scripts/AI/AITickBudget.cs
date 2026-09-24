using System;
using System.Collections.Generic;

namespace Vanta.AI
{
    /// <summary>Frame-level AI budget that keeps high-value agents responsive on constrained hardware.</summary>
    public sealed class AITickBudget
    {
        readonly int capacity;
        readonly HashSet<int> acquired = new();

        public int Used => acquired.Count;
        public int Capacity => capacity;

        public AITickBudget(int capacity) => this.capacity = Math.Max(0, capacity);

        public bool TryAcquire(int agentId, float priority)
        {
            if (acquired.Contains(agentId)) return true;
            if (acquired.Count >= capacity || priority < 0f) return false;
            acquired.Add(agentId);
            return true;
        }

        public void Reset() => acquired.Clear();
    }
}