using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.World
{
    public sealed class DistrictRuntimeSystem : MonoBehaviour
    {
        readonly Dictionary<string, DistrictDefinition> districts = new();
        readonly HashSet<string> unlocked = new();
        public string ActiveDistrictId { get; private set; }
        public event Action<string> DistrictUnlocked;
        public event Action<string> ActiveDistrictChanged;

        public void Register(DistrictDefinition definition, bool initiallyUnlocked = false)
        {
            if (!definition || string.IsNullOrWhiteSpace(definition.districtId)) return;
            districts[definition.districtId] = definition;
            if (initiallyUnlocked) Unlock(definition.districtId);
        }

        public bool IsKnown(string id) => !string.IsNullOrWhiteSpace(id) && districts.ContainsKey(id);
        public bool IsUnlocked(string id) => !string.IsNullOrWhiteSpace(id) && unlocked.Contains(id);

        public bool Unlock(string id)
        {
            if (!IsKnown(id) || !unlocked.Add(id)) return false;
            DistrictUnlocked?.Invoke(id);
            return true;
        }

        public bool SetActive(string id)
        {
            if (!IsUnlocked(id) || ActiveDistrictId == id) return false;
            ActiveDistrictId = id;
            ActiveDistrictChanged?.Invoke(id);
            return true;
        }

        public DistrictDefinition Get(string id) =>
            districts.TryGetValue(id, out var definition) ? definition : null;
    }
}