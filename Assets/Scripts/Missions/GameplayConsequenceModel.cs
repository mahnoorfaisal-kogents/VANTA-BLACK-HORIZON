using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Missions
{
    public sealed class GameplayConsequenceModel
    {
        readonly Dictionary<string,int> reputation = new();
        readonly Dictionary<string,int> territory = new();
        readonly HashSet<string> revealed = new();

        public int Cash { get; private set; }
        public float WantedHeat { get; private set; }
        public int Xp { get; private set; }

        public void Apply(MissionConsequence consequence)
        {
            if (consequence == null) return;
            Cash = Mathf.Max(0, Cash + consequence.cash);
            WantedHeat = Mathf.Clamp(WantedHeat + Mathf.Max(0f, consequence.wantedHeat), 0f, 5f);
            Xp = Mathf.Max(0, Xp + Mathf.Max(0, consequence.xp));
            if (!string.IsNullOrWhiteSpace(consequence.factionId))
                reputation[consequence.factionId] = Mathf.Clamp(GetReputation(consequence.factionId) + consequence.reputationDelta, -100, 100);
            if (!string.IsNullOrWhiteSpace(consequence.territoryId))
                territory[consequence.territoryId] = Mathf.Clamp(GetTerritoryInfluence(consequence.territoryId) + consequence.territoryDelta, -100, 100);
            if (consequence.revealIds != null)
                foreach (var id in consequence.revealIds)
                    if (!string.IsNullOrWhiteSpace(id)) revealed.Add(id);
        }

        public int GetReputation(string factionId) => factionId != null && reputation.TryGetValue(factionId, out var v) ? v : 0;
        public int GetTerritoryInfluence(string territoryId) => territoryId != null && territory.TryGetValue(territoryId, out var v) ? v : 0;
        public bool IsRevealed(string id) => !string.IsNullOrWhiteSpace(id) && revealed.Contains(id);
    }
}