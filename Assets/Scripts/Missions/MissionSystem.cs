using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Missions
{
    public sealed class MissionSystem : MonoBehaviour
    {
        public enum Status { Inactive, Active, Complete, Failed }

        [Serializable]
        public sealed class RuntimeMission
        {
            public string id;
            public Status status;
            public int objectiveIndex;
            public MissionObjectiveGraph graph = new();
            public MissionConsequence consequence;
        }

        readonly Dictionary<string, RuntimeMission> missions = new();
        public event Action<string, Status> StatusChanged;

        public string ActiveMissionId
        {
            get
            {
                foreach (var pair in missions)
                    if (pair.Value.status == Status.Active) return pair.Key;
                return null;
            }
        }

        public Status GetStatus(string id) =>
            missions.TryGetValue(id, out var m) ? m.status : Status.Inactive;

        public MissionConsequence GetConsequence(string id) =>
            missions.TryGetValue(id, out var m) ? m.consequence : null;

        public bool StartMission(MissionDefinition def)
        {
            if (!def || string.IsNullOrWhiteSpace(def.missionId)) return false;
            var runtime = new RuntimeMission
            {
                id = def.missionId,
                status = Status.Active,
                consequence = def.consequence
            };
            runtime.graph.Build(def.objectiveGraph);
            missions[def.missionId] = runtime;
            StatusChanged?.Invoke(def.missionId, Status.Active);
            return true;
        }

        public bool SetObjectiveActive(string missionId, string objectiveId) =>
            missions.TryGetValue(missionId, out var m) &&
            m.status == Status.Active &&
            m.graph.SetActive(objectiveId);

        public bool CompleteObjective(string missionId, string objectiveId)
        {
            if (!missions.TryGetValue(missionId, out var m) || m.status != Status.Active) return false;
            var completed = m.graph.Complete(objectiveId);
            if (completed) m.objectiveIndex++;
            return completed;
        }

        public bool CompleteMission(string id)
        {
            if (!missions.TryGetValue(id, out var m) || m.status != Status.Active) return false;
            if (!m.graph.CanCompleteMission()) return false;
            m.status = Status.Complete;
            StatusChanged?.Invoke(id, m.status);
            return true;
        }

        public bool FailMission(string id)
        {
            if (!missions.TryGetValue(id, out var m) || m.status != Status.Active) return false;
            m.status = Status.Failed;
            StatusChanged?.Invoke(id, m.status);
            return true;
        }
    }
}
