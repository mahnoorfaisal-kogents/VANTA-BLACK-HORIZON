using UnityEngine;
using Vanta.Systems;

namespace Vanta.Missions
{
    public sealed class MissionConsequenceRuntimeBridge : MonoBehaviour
    {
        [SerializeField] private MissionSystem missions;
        [SerializeField] private EconomySystem economy;
        [SerializeField] private WantedSystem wanted;

        private GameplayConsequenceModel state = new();

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

            state.Apply(consequence);

            if (economy && consequence.cash != 0)
                economy.AddCash(consequence.cash);

            if (wanted && consequence.wantedHeat > 0f)
                wanted.AddCrime(consequence.wantedHeat);
        }
    }
}
