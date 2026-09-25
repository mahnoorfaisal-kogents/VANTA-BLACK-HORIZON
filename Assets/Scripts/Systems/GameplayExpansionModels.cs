using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Systems
{
    public enum WorldInteractionDeviceType
    {
        TrafficLight,
        SecurityGate,
        Alarm,
        Bridge,
        Camera
    }

    public enum WorldInteractionAction
    {
        Enable,
        Disable
    }

    public sealed class WorldInteractionModel
    {
        readonly Dictionary<string, DeviceState> devices = new(StringComparer.Ordinal);

        public void Register(string id, WorldInteractionDeviceType type, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Device id is required.", nameof(id));

            devices[id] = new DeviceState(type, enabled);
        }

        public bool TryApply(string id, WorldInteractionAction action)
        {
            if (!devices.TryGetValue(id, out var state))
                return false;

            state.Enabled = action == WorldInteractionAction.Enable;
            devices[id] = state;
            return true;
        }

        public bool IsEnabled(string id) =>
            devices.TryGetValue(id, out var state) && state.Enabled;

        readonly struct DeviceState
        {
            public readonly WorldInteractionDeviceType Type;
            public bool Enabled { get; init; }

            public DeviceState(WorldInteractionDeviceType type, bool enabled)
            {
                Type = type;
                Enabled = enabled;
            }
        }
    }

    public sealed class BulletTimeModel
    {
        readonly float duration;
        readonly float cooldown;
        readonly float timeScale;
        float remaining;
        float nextAllowedTime;

        public BulletTimeModel(float duration, float cooldown, float timeScale)
        {
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration));
            if (cooldown < 0f) throw new ArgumentOutOfRangeException(nameof(cooldown));
            if (timeScale <= 0f || timeScale > 1f) throw new ArgumentOutOfRangeException(nameof(timeScale));

            this.duration = duration;
            this.cooldown = cooldown;
            this.timeScale = timeScale;
            nextAllowedTime = float.NegativeInfinity;
        }

        public bool IsActive => remaining > 0f;
        public float CurrentTimeScale => IsActive ? timeScale : 1f;

        public bool TryActivate(float now)
        {
            if (IsActive || now < nextAllowedTime)
                return false;

            remaining = duration;
            nextAllowedTime = now + duration + cooldown;
            return true;
        }

        public void Tick(float deltaTime, float now)
        {
            if (deltaTime < 0f)
                throw new ArgumentOutOfRangeException(nameof(deltaTime));

            remaining = Mathf.Max(0f, remaining - deltaTime);
            if (remaining <= 0f && now > nextAllowedTime - cooldown)
                nextAllowedTime = Mathf.Max(nextAllowedTime, now);
        }
    }

    public sealed class GrappleTraversalModel
    {
        readonly float maxDistance;

        public GrappleTraversalModel(float maxDistance)
        {
            if (maxDistance <= 0f)
                throw new ArgumentOutOfRangeException(nameof(maxDistance));
            this.maxDistance = maxDistance;
        }

        public bool IsAttached { get; private set; }
        public string AnchorId { get; private set; }

        public bool TryAttach(float distance, string anchorId)
        {
            if (string.IsNullOrWhiteSpace(anchorId) || distance < 0f || distance > maxDistance)
                return false;

            AnchorId = anchorId;
            IsAttached = true;
            return true;
        }

        public void Detach()
        {
            IsAttached = false;
            AnchorId = null;
        }
    }

    public sealed class RacingEventModel
    {
        readonly string[] checkpoints;
        readonly int lapCount;
        int nextCheckpoint;

        public RacingEventModel(string[] checkpoints, int lapCount)
        {
            if (checkpoints == null || checkpoints.Length == 0)
                throw new ArgumentException("At least one checkpoint is required.", nameof(checkpoints));
            if (lapCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(lapCount));

            this.checkpoints = (string[])checkpoints.Clone();
            this.lapCount = lapCount;
        }

        public int CompletedLaps { get; private set; }
        public bool IsComplete => CompletedLaps >= lapCount;

        public bool TryPassCheckpoint(string checkpointId)
        {
            if (IsComplete || !string.Equals(checkpoints[nextCheckpoint], checkpointId, StringComparison.Ordinal))
                return false;

            nextCheckpoint++;
            if (nextCheckpoint >= checkpoints.Length)
            {
                nextCheckpoint = 0;
                CompletedLaps++;
            }

            return true;
        }
    }
}
