using UnityEngine;
using Vanta.Systems;

namespace Vanta.Missions
{
    public sealed class MissionConsequenceRuntimeBridge : MonoBehaviour
    {
        [SerializeField] private MissionSystem missions;
        [SerializeField] private EconomySystem economy;
        [SerializeField] private WantedSystem wanted;
        [SerializeField] private MissionApproach selectedApproach = MissionApproach.Vehicle;

        private GameplayConsequenceModel state = new();
        private readonly MissionApproachModel approachModel = new();

        public MissionApproach SelectedApproach => selectedApproach;

        public void SetApproach(MissionApproach approach) => selectedApproach = approach;

        private void OnEnable()
        {
            if (missions)
                missions.StatusChanged += HandleMissionStatusChanged;
        }

        private void OnDisable()
        {
            if (missions)
                missions.StatusChanged -= HandleMissionStatusChanged;
        }

        private void HandleMissionStatusChanged(string missionId, MissionSystem.Status status)
        {
            if (status != MissionSystem.Status.Complete || !missions)
                return;

            var consequence = missions.GetConsequence(missionId);
            if (consequence == null)
                return;

            var approach = approachModel.Resolve(selectedApproach);
            var adjusted = new MissionConsequence
            {
                cash = Mathf.Max(0, Mathf.RoundToInt(consequence.cash * approach.RewardMultiplier)),
                factionId = consequence.factionId,
                reputationDelta = consequence.reputationDelta + approach.ReputationDelta,
                territoryId = consequence.territoryId,
                territoryDelta = consequence.territoryDelta,
                wantedHeat = Mathf.Max(0f, consequence.wantedHeat + approach.WantedHeat),
                xp = consequence.xp,
                revealIds = consequence.revealIds
            };

            state.Apply(adjusted);

            if (economy && adjusted.cash != 0)
                economy.AddCash(adjusted.cash);

            if (wanted && adjusted.wantedHeat > 0f)
                wanted.AddCrime(adjusted.wantedHeat);
        }
    }
}
