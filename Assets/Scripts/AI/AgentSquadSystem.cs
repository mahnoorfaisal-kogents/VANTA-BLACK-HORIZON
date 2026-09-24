using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.AI
{
    public enum SquadRole { Leader, Assault, Support, Scout }

    [Serializable]
    public readonly struct SquadMember
    {
        public readonly int Id;
        public readonly SquadRole Role;
        public readonly float Priority;
        public SquadMember(int id, SquadRole role, float priority)
        {
            Id = id; Role = role; Priority = Mathf.Clamp01(priority);
        }
    }

    /// <summary>Deterministic squad coordination data layer for coordinated pursuit, assault and support behavior.</summary>
    public sealed class AgentSquadSystem
    {
        readonly Dictionary<int, SquadMember> members = new();
        public int Count => members.Count;

        public void AddOrUpdate(SquadMember member) => members[member.Id] = member;
        public bool Remove(int id) => members.Remove(id);

        public SquadMember SelectLeader()
        {
            var selected = default(SquadMember);
            var found = false;
            foreach (var member in members.Values)
            {
                if (member.Role != SquadRole.Leader && member.Role != SquadRole.Assault) continue;
                if (!found || member.Priority > selected.Priority ||
                    (Mathf.Approximately(member.Priority, selected.Priority) && member.Id < selected.Id))
                {
                    selected = member; found = true;
                }
            }
            return selected;
        }
    }
}