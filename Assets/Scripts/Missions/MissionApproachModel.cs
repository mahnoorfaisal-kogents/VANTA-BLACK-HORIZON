using UnityEngine;

namespace Vanta.Missions
{
    public enum MissionApproach
    {
        Stealth,
        Assault,
        Vehicle
    }

    public readonly struct MissionApproachResult
    {
        public readonly float RewardMultiplier;
        public readonly float WantedHeat;
        public readonly int ReputationDelta;
        public readonly float TimePressure;

        public MissionApproachResult(float rewardMultiplier, float wantedHeat, int reputationDelta, float timePressure)
        {
            RewardMultiplier = rewardMultiplier;
            WantedHeat = wantedHeat;
            ReputationDelta = reputationDelta;
            TimePressure = timePressure;
        }
    }

    public sealed class MissionApproachModel
    {
        public MissionApproachResult Resolve(MissionApproach approach)
        {
            return approach switch
            {
                MissionApproach.Stealth => new MissionApproachResult(1.2f, 0.35f, 8, 0.15f),
                MissionApproach.Assault => new MissionApproachResult(1.05f, 1.5f, 2, 0.8f),
                MissionApproach.Vehicle => new MissionApproachResult(1.15f, 0.8f, 5, 0.5f),
                _ => new MissionApproachResult(1f, 0f, 0, 0f)
            };
        }
    }
}
