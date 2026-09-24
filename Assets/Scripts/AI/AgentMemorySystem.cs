using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.AI
{
    [Serializable]
    public sealed class AgentMemoryEntry
    {
        public string key;
        public string value;
        public float importance;
        public int tick;

        public AgentMemoryEntry Clone() => new()
        {
            key = key, value = value, importance = importance, tick = tick
        };
    }

    /// <summary>
    /// Lightweight, deterministic game-state memory inspired by retrieval-based NPC systems.
    /// It stores authored facts/events; it never calls a remote model.
    /// </summary>
    public sealed class AgentMemoryStore
    {
        readonly Dictionary<string, AgentMemoryEntry> entries = new();
        readonly int capacity;

        public int Count => entries.Count;
        public AgentMemoryStore(int capacity = 32) => this.capacity = Mathf.Max(1, capacity);

        public bool Remember(string key, string value, float importance, int tick)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) return false;
            importance = Mathf.Clamp01(importance);
            if (!entries.ContainsKey(key) && entries.Count >= capacity) ForgetLeastImportant();
            entries[key] = new AgentMemoryEntry { key = key, value = value, importance = importance, tick = tick };
            return true;
        }

        public bool TryRecall(string key, out AgentMemoryEntry entry)
        {
            if (entries.TryGetValue(key, out var found))
            {
                entry = found.Clone();
                return true;
            }
            entry = null;
            return false;
        }

        public IReadOnlyList<AgentMemoryEntry> RecallRelevant(string query, int maxResults = 4)
        {
            var result = new List<AgentMemoryEntry>();
            if (string.IsNullOrWhiteSpace(query)) return result;
            var tokens = query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in entries.Values)
            {
                var haystack = (item.key + " " + item.value).ToLowerInvariant();
                var score = 0;
                foreach (var token in tokens) if (haystack.Contains(token)) score++;
                if (score > 0) result.Add(item);
            }
            result.Sort((a, b) => b.importance.CompareTo(a.importance));
            if (result.Count > maxResults) result.RemoveRange(maxResults, result.Count - maxResults);
            return result;
        }

        void ForgetLeastImportant()
        {
            string key = null;
            var lowest = float.MaxValue;
            foreach (var item in entries)
                if (item.Value.importance < lowest) { lowest = item.Value.importance; key = item.Key; }
            if (key != null) entries.Remove(key);
        }
    }
}
