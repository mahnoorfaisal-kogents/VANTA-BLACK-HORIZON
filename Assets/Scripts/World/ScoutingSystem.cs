using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.World
{
    public sealed class ScoutingSystem : MonoBehaviour
    {
        readonly HashSet<string> revealed = new();
        public event Action<string> Revealed;
        public int ScoutCharges { get; private set; } = 3;
        public int RevealedCount => revealed.Count;

        public bool Reveal(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || revealed.Contains(id)) return false;
            revealed.Add(id);
            Revealed?.Invoke(id);
            return true;
        }

        public bool UseScout(IEnumerable<string> informationIds)
        {
            if (ScoutCharges <= 0 || informationIds == null) return false;
            var changed = false;
            foreach (var id in informationIds) changed |= Reveal(id);
            if (changed) ScoutCharges--;
            return changed;
        }

        public void RefillScouts(int amount)
        {
            if (amount > 0) ScoutCharges += amount;
        }

        public bool IsRevealed(string id) => !string.IsNullOrWhiteSpace(id) && revealed.Contains(id);
    }
}