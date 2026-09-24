using System;
using UnityEngine;

namespace Vanta.Missions
{
    [Serializable]
    public sealed class MissionConsequence
    {
        public int cash;
        public string factionId;
        public int reputationDelta;
        public string territoryId;
        public int territoryDelta;
        public float wantedHeat;
        public int xp;
        public string[] revealIds;
    }

    public readonly struct MissionConsequenceResult
    {
        public readonly int Cash;
        public readonly string FactionId;
        public readonly int ReputationDelta;
        public readonly string TerritoryId;
        public readonly int TerritoryDelta;
        public readonly float WantedHeat;
        public readonly int Xp;
        public readonly string[] RevealIds;
        public MissionConsequenceResult(MissionConsequence c)
        {
            Cash=c?.cash??0; FactionId=c?.factionId; ReputationDelta=c?.reputationDelta??0;
            TerritoryId=c?.territoryId; TerritoryDelta=c?.territoryDelta??0; WantedHeat=c?.wantedHeat??0f; Xp=c?.xp??0;
            RevealIds=c?.revealIds??Array.Empty<string>();
        }
    }

    public sealed class MissionConsequenceSystem : MonoBehaviour
    {
        public MissionConsequenceResult Resolve(MissionConsequence consequence) => new(consequence);
    }
}