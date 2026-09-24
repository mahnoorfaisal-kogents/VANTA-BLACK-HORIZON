using System;
using UnityEngine;

namespace Vanta.AI
{
    public enum SquadCommand
    {
        Hold,
        Advance,
        Flank,
        Suppress,
        Search,
        Retreat,
        Regroup
    }

    [Serializable]
    public readonly struct SquadCommandContext
    {
        public readonly SquadRole Role;
        public readonly float Threat;
        public readonly float TargetDistance;
        public readonly bool TargetVisible;
        public readonly bool HasLeader;
        public readonly bool LeaderVisible;
        public SquadCommandContext(SquadRole role, float threat, float targetDistance,
            bool targetVisible, bool hasLeader, bool leaderVisible)
        {
            Role = role;
            Threat = Mathf.Clamp01(threat);
            TargetDistance = Mathf.Max(0f, targetDistance);
            TargetVisible = targetVisible;
            HasLeader = hasLeader;
            LeaderVisible = leaderVisible;
        }
    }

    public readonly struct SquadCommandResult
    {
        public readonly SquadCommand Command;
        public readonly float Utility;
        public SquadCommandResult(SquadCommand command, float utility)
        {
            Command = command;
            Utility = Mathf.Clamp01(utility);
        }
    }

    /// <summary>Deterministic command arbitration. It produces tactical orders only; controllers remain responsible for execution.</summary>
    public static class AgentSquadCommandSystem
    {
        public static SquadCommandResult Evaluate(SquadCommandContext context)
        {
            if (!context.HasLeader)
                return new SquadCommandResult(SquadCommand.Regroup, 0.9f);
            if (context.Threat >= 0.85f)
                return new SquadCommandResult(context.Role == SquadRole.Scout ? SquadCommand.Retreat : SquadCommand.Regroup, context.Threat);
            if (!context.TargetVisible)
                return new SquadCommandResult(SquadCommand.Search, 0.55f + context.Threat * 0.25f);

            var distanceFactor = Mathf.Clamp01(context.TargetDistance / 30f);
            return context.Role switch
            {
                SquadRole.Leader => new SquadCommandResult(SquadCommand.Advance, 0.65f + (1f - distanceFactor) * 0.25f),
                SquadRole.Assault => new SquadCommandResult(SquadCommand.Flank, 0.7f + distanceFactor * 0.2f),
                SquadRole.Support => new SquadCommandResult(SquadCommand.Suppress, 0.72f),
                _ => new SquadCommandResult(SquadCommand.Flank, 0.6f + distanceFactor * 0.2f)
            };
        }
    }
}
