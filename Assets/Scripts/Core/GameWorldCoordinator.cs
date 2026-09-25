using UnityEngine;
using Vanta.AI;
using Vanta.Missions;
using Vanta.Map;
using Vanta.Systems;
using Vanta.World;

namespace Vanta.Core
{
    public sealed class GameWorldCoordinator : MonoBehaviour
    {
        [SerializeField] WantedSystem wanted;
        [SerializeField] PoliceEscalationSystem policeEscalation;
        [SerializeField] PolicePursuitCoordinator pursuit;
        [SerializeField] MissionSystem missions;
        [SerializeField] MissionConsequenceSystem consequences;
        [SerializeField] FactionSystem factions;
        [SerializeField] FactionTerritorySystem territories;
        [SerializeField] EconomySystem economy;
        [SerializeField] ProgressionSystem progression;
        [SerializeField] IntelMapSystem intel;
        [SerializeField] WorldInteractionDevice[] worldInteractionDevices;

        readonly WorldInteractionModel worldInteractions = new();

        void Awake()
        {
            if (wanted) wanted.LevelChanged += OnWantedChanged;
            if (worldInteractionDevices != null)
                foreach (var device in worldInteractionDevices)
                    if (device) device.Initialize(worldInteractions);
            if (missions) missions.StatusChanged += OnMissionStatusChanged;
        }

        void OnDestroy()
        {
            if (wanted) wanted.LevelChanged -= OnWantedChanged;
            if (missions) missions.StatusChanged -= OnMissionStatusChanged;
        }

        void Start()
        {
            if (wanted) OnWantedChanged(wanted.Level);
        }

        void OnWantedChanged(int level)
        {
            if (policeEscalation) policeEscalation.UpdateFromWantedLevel(level);
            if (pursuit) pursuit.Update(level);
        }

        void OnMissionStatusChanged(string missionId, MissionSystem.Status status)
        {
            if (status != MissionSystem.Status.Complete || !missions || !consequences) return;
            var consequence = missions.GetConsequence(missionId);
            if (consequence == null) return;
            var result = consequences.Resolve(consequence);
            if (economy && result.Cash != 0) economy.AddCash(result.Cash);
            if (progression && result.Xp > 0) progression.AddXp(result.Xp);
            if (wanted && result.WantedHeat > 0) wanted.AddCrime(result.WantedHeat);
            if (factions && !string.IsNullOrWhiteSpace(result.FactionId) && result.ReputationDelta != 0)
                factions.ChangeReputation(result.FactionId, result.ReputationDelta);
            if (territories && !string.IsNullOrWhiteSpace(result.TerritoryId) && result.TerritoryDelta != 0)
                territories.ApplyInfluence(result.TerritoryId, result.TerritoryDelta, result.FactionId);
            if (intel && result.RevealIds != null)
                foreach (var id in result.RevealIds)
                    intel.Reveal(id, WorldMarker.MarkerType.Mission);
        }
    }
}