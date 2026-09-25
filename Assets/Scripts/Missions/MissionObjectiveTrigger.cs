using UnityEngine;

namespace Vanta.Missions
{
    /// <summary>Minimal world interaction bridge for vertical-slice objectives. It only advances the configured mission state.</summary>
    [RequireComponent(typeof(Collider))]
    public sealed class MissionObjectiveTrigger : MonoBehaviour
    {
        [SerializeField] MissionSystem missionSystem;
        [SerializeField] string missionId;
        [SerializeField] string objectiveId;
        [SerializeField] bool consumeOnComplete = true;
        bool consumed;

        void OnTriggerEnter(Collider other)
        {
            if (consumed || !missionSystem || !other || !other.CompareTag("Player")) return;
            if (missionSystem.CompleteObjective(missionId, objectiveId))
            {
                consumed = consumeOnComplete;
                var definition = GetComponentInParent<MissionObjectiveTriggerSet>();
                if (definition) definition.TryActivateNext(objectiveId);
            }
        }
    }

    public sealed class MissionObjectiveTriggerSet : MonoBehaviour
    {
        [SerializeField] MissionSystem missionSystem;
        [SerializeField] string missionId;
        [SerializeField] string[] objectiveIds;
        public void Configure(MissionSystem system, string id, string[] ids)
        {
            missionSystem = system;
            missionId = id;
            objectiveIds = ids;
        }

        public void TryActivateNext(string completedId)
        {
            if (!missionSystem || objectiveIds == null || objectiveIds.Length == 0) return;

            for (var i = 0; i < objectiveIds.Length - 1; i++)
            {
                if (objectiveIds[i] == completedId)
                {
                    missionSystem.SetObjectiveActive(missionId, objectiveIds[i + 1]);
                    return;
                }
            }

            if (objectiveIds[objectiveIds.Length - 1] == completedId)
                missionSystem.CompleteMission(missionId);
        }
    }
}
