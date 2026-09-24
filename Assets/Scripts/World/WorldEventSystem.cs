using System;
using UnityEngine;

namespace Vanta.World
{
    public enum WorldEventState { Scheduled, Active, Resolved, Failed }

    [Serializable]
    public sealed class WorldEventDefinition
    {
        public string eventId;
        public string districtId;
        public float startHour;
        public float durationHours = 1f;
        public int rewardCash;
    }

    public sealed class WorldEventRuntime
    {
        public WorldEventState State { get; private set; } = WorldEventState.Scheduled;
        public bool TryActivate()
        {
            if (State != WorldEventState.Scheduled) return false;
            State = WorldEventState.Active; return true;
        }
        public bool TryResolve()
        {
            if (State != WorldEventState.Active) return false;
            State = WorldEventState.Resolved; return true;
        }
        public bool TryFail()
        {
            if (State != WorldEventState.Active) return false;
            State = WorldEventState.Failed; return true;
        }
    }

    public sealed class WorldEventSystem : MonoBehaviour
    {
        public event Action<string, WorldEventState> StateChanged;
        public WorldEventRuntime CreateRuntime(WorldEventDefinition definition) => new();
        public bool Activate(string id, WorldEventRuntime runtime)
        {
            var changed = runtime != null && runtime.TryActivate();
            if (changed) StateChanged?.Invoke(id, runtime.State);
            return changed;
        }
        public bool Resolve(string id, WorldEventRuntime runtime)
        {
            var changed = runtime != null && runtime.TryResolve();
            if (changed) StateChanged?.Invoke(id, runtime.State);
            return changed;
        }
        public bool Fail(string id, WorldEventRuntime runtime)
        {
            var changed = runtime != null && runtime.TryFail();
            if (changed) StateChanged?.Invoke(id, runtime.State);
            return changed;
        }
    }
}