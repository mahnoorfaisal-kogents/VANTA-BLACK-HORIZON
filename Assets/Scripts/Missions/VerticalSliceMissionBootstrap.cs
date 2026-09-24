using UnityEngine;

namespace Vanta.Missions
{
    /// <summary>Starts the authored vertical-slice mission and activates its first objective once the scene is ready.</summary>
    public sealed class VerticalSliceMissionBootstrap : MonoBehaviour
    {
        [SerializeField] MissionSystem missionSystem;
        [SerializeField] MissionDefinition missionDefinition;
        bool started;
        void Start()
        {
            if (started || !missionSystem || !missionDefinition) return;
            started = missionSystem.StartMission(missionDefinition);
            if (started && missionDefinition.objectiveGraph != null && missionDefinition.objectiveGraph.Length > 0)
                missionSystem.SetObjectiveActive(missionDefinition.missionId, missionDefinition.objectiveGraph[0].id);
        }
    }
}
