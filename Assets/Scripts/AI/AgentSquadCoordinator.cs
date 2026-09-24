using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.AI
{
    public readonly struct SquadAssignment
    {
        public readonly int Id;
        public readonly SquadRole Role;
        public readonly float Priority;
        public SquadAssignment(int id, SquadRole role, float priority) { Id=id; Role=role; Priority=priority; }
    }

    /// <summary>Deterministic runtime squad coordinator. Highest priority leads; remaining members fill tactical roles.</summary>
    public sealed class AgentSquadCoordinator
    {
        readonly List<SquadMember> members = new();
        readonly int maxSize;
        public AgentSquadCoordinator(int maxSize = 8) => this.maxSize = Mathf.Max(1, maxSize);

        public void Add(int id, float priority)
        {
            if (members.Count >= maxSize || members.Exists(x => x.Id == id)) return;
            members.Add(new SquadMember(id, SquadRole.Assault, priority));
        }

        public IReadOnlyList<SquadAssignment> BuildAssignments()
        {
            var ordered = new List<SquadMember>(members);
            ordered.Sort((a,b) => b.Priority.CompareTo(a.Priority));
            var result = new List<SquadAssignment>(ordered.Count);
            for (var i=0;i<ordered.Count;i++)
            {
                var role = i switch { 0 => SquadRole.Leader, 1 => SquadRole.Assault, 2 => SquadRole.Support, _ => SquadRole.Scout };
                result.Add(new SquadAssignment(ordered[i].Id, role, ordered[i].Priority));
            }
            return result;
        }
    }

    public sealed class AgentSquadCoordinatorComponent : MonoBehaviour
    {
        [SerializeField, Min(1)] int maxSize = 8;
        [SerializeField] float refreshSeconds = 1f;
        AgentSquadCoordinator coordinator;
        float timer;
        void Awake() => coordinator = new AgentSquadCoordinator(maxSize);

        public IReadOnlyList<SquadAssignment> CurrentAssignments { get; private set; } = Array.Empty<SquadAssignment>();

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            timer = Mathf.Max(0.1f, refreshSeconds);
            coordinator.Clear();
            var brains = GetComponentsInChildren<VantaAgentBrain>(true);
            var player = GameObject.FindGameObjectWithTag("Player")?.transform;
            for (var i = 0; i < brains.Length; i++)
            {
                if (!brains[i] || (player && brains[i].transform == player)) continue;
                var distance = player ? Vector3.Distance(transform.position, player.position) : 0f;
                var priority = Mathf.Clamp01(1f - distance / 100f) + (brains[i].LastAction == AgentAction.Pursue ? 0.2f : 0f);
                coordinator.Add(brains[i].GetInstanceID(), Mathf.Clamp01(priority));
            }
            CurrentAssignments = coordinator.BuildAssignments();
        }

        public void AddMember(int id, float priority) => coordinator.Add(id, priority);
    }
}
