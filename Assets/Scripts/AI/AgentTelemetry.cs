using System;
using System.Collections.Generic;

namespace Vanta.AI
{
    public readonly struct AgentDecisionTrace
    {
        public readonly int Tick;
        public readonly AgentAction Action;
        public readonly float Threat;
        public AgentDecisionTrace(int tick, AgentAction action, float threat)
        {
            Tick = tick; Action = action; Threat = threat;
        }
    }

    /// <summary>Bounded diagnostic history for balancing and debugging; contains no external telemetry dependency.</summary>
    public sealed class AgentTelemetry
    {
        readonly int capacity;
        readonly Queue<AgentDecisionTrace> traces = new();

        public AgentTelemetry(int capacity = 64) => this.capacity = Math.Max(1, capacity);
        public int Count => traces.Count;

        public void Record(AgentDecisionTrace trace)
        {
            while (traces.Count >= capacity) traces.Dequeue();
            traces.Enqueue(trace);
        }

        public IReadOnlyCollection<AgentDecisionTrace> Snapshot() =>
            new List<AgentDecisionTrace>(traces);
    }
}