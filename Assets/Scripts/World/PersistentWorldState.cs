using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.World
{
    [Serializable]
    public sealed class TerritoryInfluenceEntry
    {
        public string districtId;
        public int influence;
    }

    [Serializable]
    public sealed class WorldStateSnapshot
    {
        public float timeOfDay;
        public WeatherSystem.WeatherState weather;
        public List<string> discovered = new();
        public List<string> completedMissions = new();
        public List<TerritoryInfluenceEntry> territoryInfluenceEntries = new();
    }

    public sealed class PersistentWorldState : MonoBehaviour
    {
        public WorldStateSnapshot Capture(float time, WeatherSystem.WeatherState weather,
            IEnumerable<string> discoveredIds, IEnumerable<string> missions,
            IDictionary<string, int> territory)
        {
            var snapshot = new WorldStateSnapshot
            {
                timeOfDay = time,
                weather = weather,
                discovered = discoveredIds == null ? new List<string>() : new List<string>(discoveredIds),
                completedMissions = missions == null ? new List<string>() : new List<string>(missions)
            };

            if (territory != null)
            {
                foreach (var pair in territory)
                    snapshot.territoryInfluenceEntries.Add(new TerritoryInfluenceEntry
                    {
                        districtId = pair.Key,
                        influence = pair.Value
                    });
            }

            return snapshot;
        }
    }
}
