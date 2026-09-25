using System;
using Vanta.Systems;

namespace Vanta.Racing
{
    public sealed class RacingActivityModel
    {
        readonly RacingEventModel race;
        readonly int baseReward;
        bool rewarded;

        public RacingActivityModel(string[] checkpoints, int lapCount, int baseReward)
        {
            if (baseReward < 0) throw new ArgumentOutOfRangeException(nameof(baseReward));
            race = new RacingEventModel(checkpoints, lapCount);
            this.baseReward = baseReward;
        }

        public int CompletedLaps => race.CompletedLaps;
        public bool IsComplete => race.IsComplete;
        public int RewardCash { get; private set; }

        public bool Pass(string checkpointId, float rewardMultiplier = 1f)
        {
            if (race.IsComplete || rewardMultiplier < 0f)
                return false;

            if (!race.TryPassCheckpoint(checkpointId))
                return false;

            if (race.IsComplete && !rewarded)
            {
                RewardCash = Math.Max(0, (int)Math.Round(baseReward * rewardMultiplier));
                rewarded = true;
            }

            return true;
        }
    }
}
