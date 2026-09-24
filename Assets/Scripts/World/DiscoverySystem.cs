using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.World
{
    public sealed class DiscoverySystem : MonoBehaviour
    {
        private readonly HashSet<string> discovered = new();
        public event Action<string> Discovered;

        public bool IsDiscovered(string id) =>
            !string.IsNullOrWhiteSpace(id) && discovered.Contains(id);

        public bool Discover(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !discovered.Add(id)) return false;
            Discovered?.Invoke(id);
            return true;
        }

        public int Count => discovered.Count;
    }
}
