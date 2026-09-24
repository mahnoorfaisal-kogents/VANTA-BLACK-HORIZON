using UnityEngine;
using Vanta.Systems;

namespace Vanta.Systems
{
    public enum WorldMissionAccess { Available, Restricted }

    public readonly struct FactionWorldEffect
    {
        public readonly float DangerMultiplier;
        public readonly WorldMissionAccess MissionAccess;
        public readonly float CivilianThreat;
        public FactionWorldEffect(float danger, WorldMissionAccess access, float threat)
        { DangerMultiplier=danger; MissionAccess=access; CivilianThreat=threat; }
    }

    public static class FactionWorldEffects
    {
        public static FactionWorldEffect Evaluate(TerritoryControl control, int controlValue)
        {
            var danger = control switch
            {
                TerritoryControl.Hostile => 1.5f,
                TerritoryControl.Contested => 1.25f,
                TerritoryControl.Allied => .75f,
                _ => 1f
            };
            var access = control == TerritoryControl.Hostile && controlValue >= 60
                ? WorldMissionAccess.Restricted : WorldMissionAccess.Available;
            var threat = control == TerritoryControl.Hostile ? 1f : control == TerritoryControl.Contested ? .5f : .2f;
            return new FactionWorldEffect(danger, access, threat);
        }
    }
}