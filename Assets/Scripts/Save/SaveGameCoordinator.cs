using UnityEngine;
using Vanta.Core;
using Vanta.Missions;
using Vanta.Systems;
using Vanta.World;

namespace Vanta.Save
{
    public sealed class SaveGameCoordinator : MonoBehaviour
    {
        [SerializeField] SaveSystem saveSystem;
        [SerializeField] Transform player;
        [SerializeField] EconomySystem economy;
        [SerializeField] WantedSystem wanted;
        [SerializeField] ProgressionSystem progression;
        [SerializeField] WorldTimeSystem worldTime;
        [SerializeField] MissionSystem missions;
        [SerializeField] MissionDefinition[] missionDefinitions;

        public bool SaveSlot(string slot)
        {
            if (!saveSystem || !player) return false;
            return saveSystem.Save(slot, new SaveData
            {
                playerPosition = player.position,
                cash = economy ? economy.Cash : 0,
                wantedLevel = wanted ? wanted.Level : 0,
                wantedHeat = wanted ? wanted.Heat : 0f,
                xp = progression ? progression.Xp : 0,
                timeOfDay = worldTime ? worldTime.TimeOfDay : 8f,
                activeMission = missions ? missions.ActiveMissionId : null,
                missions = missions ? missions.CaptureSaveState() : new System.Collections.Generic.List<MissionSaveState>()
            });
        }

        public bool LoadSlot(string slot)
        {
            if (!saveSystem || !player) return false;
            var data = saveSystem.Load(slot);
            if (data == null) return false;

            player.position = data.playerPosition;
            if (economy) economy.SetCashForLoad(data.cash);
            if (wanted) wanted.SetHeatForLoad(data.wantedHeat);
            if (progression) progression.SetXpForLoad(data.xp);
            if (worldTime) worldTime.SetTimeForLoad(data.timeOfDay);
            if (missions && data.missions != null)
                missions.RestoreSaveState(data.missions, missionDefinitions);
            return true;
        }
    }
}