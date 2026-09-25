using UnityEngine;
using Vanta.Systems;
using Vanta.World;

namespace Vanta.Racing
{
    public sealed class RacingActivitySystem : MonoBehaviour
    {
        [SerializeField] private string[] checkpoints;
        [SerializeField, Min(1)] private int laps = 2;
        [SerializeField, Min(0)] private int baseReward = 500;
        [SerializeField] private EconomySystem economy;
        [SerializeField] private WorldRiskRuntimeSystem worldRisk;

        RacingActivityModel model;

        public bool IsComplete => model != null && model.IsComplete;
        public int CompletedLaps => model?.CompletedLaps ?? 0;

        void Awake()
        {
            if (checkpoints != null && checkpoints.Length > 0)
                model = new RacingActivityModel(checkpoints, laps, baseReward);
        }

        public bool PassCheckpoint(string checkpointId)
        {
            if (model == null) return false;

            var multiplier = worldRisk ? worldRisk.Current.RewardMultiplier : 1f;
            var wasComplete = model.IsComplete;
            if (!model.Pass(checkpointId, multiplier))
                return false;

            if (!wasComplete && model.IsComplete && economy && model.RewardCash > 0)
                economy.AddCash(model.RewardCash);

            return true;
        }
    }
}
