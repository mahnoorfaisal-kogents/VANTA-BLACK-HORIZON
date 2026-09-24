using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Missions
{
    [Serializable]
    public sealed class MissionObjectiveSaveState
    {
        public string id;
        public int status;
    }

    [Serializable]
    public sealed class MissionSaveState
    {
        public string id;
        public int status;
        public int objectiveIndex;
        public List<MissionObjectiveSaveState> objectives = new();
    }

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

        public ObjectiveStatus GetObjectiveStatus(string missionId, string objectiveId) =>
            missions.TryGetValue(missionId, out var mission)
                ? mission.graph.GetStatus(objectiveId)
                : ObjectiveStatus.Locked;

        public List<MissionSaveState> CaptureSaveState()
        {
            var snapshot = new List<MissionSaveState>(missions.Count);
            foreach (var pair in missions)
            {
                var mission = pair.Value;
                var state = new MissionSaveState
                {
                    id = mission.id,
                    status = (int)mission.status,
                    objectiveIndex = mission.objectiveIndex
                };
                foreach (var node in mission.graph.Nodes.Values)
                    state.objectives.Add(new MissionObjectiveSaveState { id = node.id, status = (int)node.status });
                snapshot.Add(state);
            }
            return snapshot;
        }

        public bool RestoreSaveState(List<MissionSaveState> snapshot, IEnumerable<MissionDefinition> definitions)
        {
            if (snapshot == null) return false;
            var lookup = new Dictionary<string, MissionDefinition>();
            var ambiguousIds = new HashSet<string>();
            foreach (var definition in definitions ?? Array.Empty<MissionDefinition>())
            {
                if (!definition || string.IsNullOrWhiteSpace(definition.missionId)) continue;
                if (ambiguousIds.Contains(definition.missionId)) continue;
                if (!lookup.TryAdd(definition.missionId, definition))
                {
                    lookup.Remove(definition.missionId);
                    ambiguousIds.Add(definition.missionId);
                }
            }

            missions.Clear();
            var restoredIds = new HashSet<string>();
            foreach (var saved in snapshot)
            {
                if (saved == null || string.IsNullOrWhiteSpace(saved.id) ||
                    !restoredIds.Add(saved.id) || ambiguousIds.Contains(saved.id) ||
                    !lookup.TryGetValue(saved.id, out var definition))
                    continue;

                var graph = new MissionObjectiveGraph();
                if (!graph.TryBuild(definition.objectiveGraph)) continue;
                graph.RestoreStatuses(saved.objectives);

                missions.Add(saved.id, new RuntimeMission
                {
                    id = saved.id,
                    status = (Status)Mathf.Clamp(saved.status, (int)Status.Inactive, (int)Status.Failed),
                    objectiveIndex = Mathf.Max(0, saved.objectiveIndex),
                    graph = graph,
                    consequence = definition.consequence
                });
            }

            return true;
        }

        public bool StartMission(MissionDefinition def)
        {
            if (!def || string.IsNullOrWhiteSpace(def.missionId) || missions.ContainsKey(def.missionId)) return false;
            var graph = new MissionObjectiveGraph();
            if (!graph.TryBuild(def.objectiveGraph)) return false;

            missions.Add(def.missionId, new RuntimeMission
            {
                id = def.missionId,
                status = Status.Active,
                graph = graph,
                consequence = def.consequence
            });
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
