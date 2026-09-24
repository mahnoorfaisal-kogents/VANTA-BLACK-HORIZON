using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Systems
{
    public readonly struct InteractionOption
    {
        public readonly string Id;
        public readonly string Label;
        public InteractionOption(string id, string label) { Id = id; Label = label; }
    }

    public sealed class InteractionSystem : MonoBehaviour
    {
        readonly Dictionary<string, InteractionOption> options = new();
        public event Action<string> Interacted;

        public bool Register(string id, string label)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(label)) return false;
            options[id] = new InteractionOption(id, label);
            return true;
        }

        public bool TryInteract(string id)
        {
            if (!options.ContainsKey(id)) return false;
            Interacted?.Invoke(id);
            return true;
        }

        public bool TryGet(string id, out InteractionOption option) => options.TryGetValue(id, out option);
        public int Count => options.Count;
    }
}