using System;
using UnityEngine;

namespace Vanta.AI
{
    /// <summary>Small deterministic blackboard for propagating authored facts between cooperating agents.</summary>
    public sealed class AgentKnowledgePropagationSystem
    {
        public event Action<string, string> FactShared;

        public bool Share(AgentKnowledgeBase source, AgentKnowledgeBase destination, string key)
        {
            if (source == null || destination == null || string.IsNullOrWhiteSpace(key)) return false;
            if (!source.TryGetFact(key, out var value)) return false;
            destination.AddFact(key, value, 0.7f);
            FactShared?.Invoke(key, value);
            return true;
        }
    }
}
