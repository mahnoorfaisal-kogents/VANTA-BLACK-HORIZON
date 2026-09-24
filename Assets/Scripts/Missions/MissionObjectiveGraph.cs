using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Missions
{
    public enum ObjectiveStatus { Locked, Available, Active, Complete, Failed }

    [Serializable]
    public sealed class MissionObjectiveNode
    {
        public string id;
        public string title;
        public string[] prerequisites;
        public string[] approaches;
        public ObjectiveStatus status = ObjectiveStatus.Locked;
        public bool Optional;
    }

    public sealed class MissionObjectiveGraph
    {
        readonly Dictionary<string, MissionObjectiveNode> nodes = new();
        public IReadOnlyDictionary<string, MissionObjectiveNode> Nodes => nodes;

        public void Build(IEnumerable<MissionObjectiveNode> source)
        {
            nodes.Clear();
            foreach (var node in source ?? Array.Empty<MissionObjectiveNode>())
            {
                if (node == null || string.IsNullOrWhiteSpace(node.id)) continue;
                nodes[node.id] = new MissionObjectiveNode
                {
                    id = node.id,
                    title = node.title,
                    prerequisites = node.prerequisites == null ? Array.Empty<string>() : (string[])node.prerequisites.Clone(),
                    approaches = node.approaches == null ? Array.Empty<string>() : (string[])node.approaches.Clone(),
                    status = ObjectiveStatus.Locked,
                    Optional = node.Optional
                };
            }
            RefreshAvailability();
        }

        public bool SetActive(string id)
        {
            if (!nodes.TryGetValue(id, out var node) || node.status != ObjectiveStatus.Available) return false;
            node.status = ObjectiveStatus.Active;
            return true;
        }

        public bool Complete(string id)
        {
            if (!nodes.TryGetValue(id, out var node) ||
                (node.status != ObjectiveStatus.Active && node.status != ObjectiveStatus.Available)) return false;
            node.status = ObjectiveStatus.Complete;
            RefreshAvailability();
            return true;
        }

        public bool Fail(string id)
        {
            if (!nodes.TryGetValue(id, out var node) || node.status == ObjectiveStatus.Complete) return false;
            node.status = ObjectiveStatus.Failed;
            return true;
        }

        public bool ArePrerequisitesComplete(MissionObjectiveNode node)
        {
            if (node.prerequisites == null || node.prerequisites.Length == 0) return true;
            foreach (var prerequisite in node.prerequisites)
                if (!nodes.TryGetValue(prerequisite, out var dependency) || dependency.status != ObjectiveStatus.Complete)
                    return false;
            return true;
        }

        public bool SupportsApproach(string id, string approach)
        {
            return nodes.TryGetValue(id, out var node) &&
                   (node.approaches == null || node.approaches.Length == 0 ||
                    Array.Exists(node.approaches, x => string.Equals(x, approach, StringComparison.OrdinalIgnoreCase)));
        }

        void RefreshAvailability()
        {
            foreach (var node in nodes.Values)
                if (node.status == ObjectiveStatus.Locked && ArePrerequisitesComplete(node))
                    node.status = ObjectiveStatus.Available;
        }
    }
}