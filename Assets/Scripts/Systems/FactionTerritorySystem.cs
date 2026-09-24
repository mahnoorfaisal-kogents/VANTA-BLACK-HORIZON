using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Systems
{
    public enum TerritoryControl { Neutral, Allied, Contested, Hostile }

    [Serializable]
    public sealed class TerritoryZone
    {
        public string zoneId;
        public string controllingFaction;
        [Range(-100,100)] public int controlValue;
        public TerritoryControl Control { get; internal set; }
    }

    public sealed class FactionTerritorySystem : MonoBehaviour
    {
        readonly Dictionary<string, TerritoryZone> zones = new();
        public event Action<string, TerritoryControl> ControlChanged;

        public void Register(TerritoryZone zone)
        {
            if (zone == null || string.IsNullOrWhiteSpace(zone.zoneId)) return;
            zones[zone.zoneId] = zone;
            Recalculate(zone);
        }

        public TerritoryZone Get(string zoneId) => zones.TryGetValue(zoneId, out var zone) ? zone : null;

        public void ApplyInfluence(string zoneId, int delta, string faction)
        {
            if (!zones.TryGetValue(zoneId, out var zone) || string.IsNullOrWhiteSpace(faction)) return;
            zone.controllingFaction = faction;
            zone.controlValue = Mathf.Clamp(zone.controlValue + delta, -100, 100);
            Recalculate(zone);
        }

        static TerritoryControl Resolve(int value)
        {
            if (value >= 60) return TerritoryControl.Allied;
            if (value <= -60) return TerritoryControl.Hostile;
            if (Mathf.Abs(value) >= 20) return TerritoryControl.Contested;
            return TerritoryControl.Neutral;
        }

        void Recalculate(TerritoryZone zone)
        {
            var next = Resolve(zone.controlValue);
            if (next == zone.Control) return;
            zone.Control = next;
            ControlChanged?.Invoke(zone.zoneId, next);
        }
    }
}