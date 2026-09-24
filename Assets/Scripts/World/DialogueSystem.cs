using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.World
{
    [Serializable]
    public sealed class DialogueChoice
    {
        public string id;
        public string text;
        public string nextNodeId;
        public int requiredReputation;
    }

    [Serializable]
    public sealed class DialogueNode
    {
        public string id;
        [TextArea] public string text;
        public DialogueChoice[] choices;
    }

    public sealed class DialogueRuntime
    {
        readonly Dictionary<string, DialogueNode> nodes = new();
        public string CurrentNodeId { get; private set; }

        public void Build(IEnumerable<DialogueNode> source)
        {
            nodes.Clear();
            foreach (var node in source ?? Array.Empty<DialogueNode>())
                if (node != null && !string.IsNullOrWhiteSpace(node.id))
                    nodes[node.id] = node;
        }

        public bool Start(string nodeId)
        {
            if (!nodes.ContainsKey(nodeId)) return false;
            CurrentNodeId = nodeId;
            return true;
        }

        public bool Choose(string choiceId, int reputation)
        {
            if (!nodes.TryGetValue(CurrentNodeId, out var node) || node.choices == null) return false;
            foreach (var choice in node.choices)
            {
                if (choice == null || choice.id != choiceId || reputation < choice.requiredReputation) continue;
                if (!nodes.ContainsKey(choice.nextNodeId)) return false;
                CurrentNodeId = choice.nextNodeId;
                return true;
            }
            return false;
        }

        public DialogueNode Current =>
            CurrentNodeId != null && nodes.TryGetValue(CurrentNodeId, out var node) ? node : null;
    }
}