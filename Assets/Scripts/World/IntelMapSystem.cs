using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.World
{
    public sealed class IntelMapSystem : MonoBehaviour
    {
        readonly Dictionary<string, WorldMarker.MarkerType> markers = new();
        public event Action<string> MarkerRevealed;
        public int RevealedCount => markers.Count;
        public bool Reveal(string id, WorldMarker.MarkerType type)
        {
            if (string.IsNullOrWhiteSpace(id) || markers.ContainsKey(id)) return false;
            markers[id] = type; MarkerRevealed?.Invoke(id); return true;
        }
        public bool IsRevealed(string id) => !string.IsNullOrWhiteSpace(id) && markers.ContainsKey(id);
        public bool TryGetType(string id, out WorldMarker.MarkerType type) => markers.TryGetValue(id, out type);
    }
}