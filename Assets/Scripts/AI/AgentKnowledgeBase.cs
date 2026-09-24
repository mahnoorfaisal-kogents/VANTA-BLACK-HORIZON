using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.AI
{
    [Serializable]
    public sealed class AgentKnowledgeFact
    {
        public string key;
        public string value;
        public float confidence;
    }

    /// <summary>Authored world-knowledge grounding. No network or model dependency.</summary>
    public sealed class AgentKnowledgeBase
    {
        readonly Dictionary<string, AgentKnowledgeFact> facts = new();

        public int Count => facts.Count;

        public void AddFact(string key, string value, float confidence = 1f)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) return;
            facts[key] = new AgentKnowledgeFact
            {
                key = key,
                value = value,
                confidence = Mathf.Clamp01(confidence)
            };
        }

        public bool TryGetFact(string key, out string value)
        {
            if (facts.TryGetValue(key, out var fact))
            {
                value = fact.value;
                return true;
            }
            value = null;
            return false;
        }

        public IReadOnlyList<AgentKnowledgeFact> Facts()
        {
            var result = new List<AgentKnowledgeFact>(facts.Count);
            foreach (var fact in facts.Values) result.Add(fact);
            result.Sort((a, b) => b.confidence.CompareTo(a.confidence));
            return result;
        }
    }
}