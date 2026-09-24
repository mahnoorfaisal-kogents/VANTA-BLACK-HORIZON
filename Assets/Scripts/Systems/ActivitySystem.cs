using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Systems
{
    public enum ActivityState { Locked, Available, Active, Complete, Failed }

    [Serializable]
    public sealed class ActivityDefinition
    {
        public string activityId;
        public string districtId;
        public int requiredLevel = 1;
        public int requiredReputation;
        public string requiredFactionId;
        public int rewardCash;
        public int rewardXp;
        public float rewardWantedHeat;
    }

    public sealed class ActivityRuntime
    {
        public ActivityState State { get; private set; }
        public ActivityRuntime(ActivityState initial) => State = initial;
        public bool Start()
        {
            if (State != ActivityState.Available) return false;
            State = ActivityState.Active;
            return true;
        }
        public bool Complete()
        {
            if (State != ActivityState.Active) return false;
            State = ActivityState.Complete;
            return true;
        }
        public bool Fail()
        {
            if (State != ActivityState.Active) return false;
            State = ActivityState.Failed;
            return true;
        }
    }

    public sealed class ActivitySystem : MonoBehaviour
    {
        readonly Dictionary<string, ActivityRuntime> runtimes = new();
        public event Action<string, ActivityState> StateChanged;

        public ActivityRuntime Register(ActivityDefinition definition, int playerLevel, int factionReputation)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.activityId)) return null;
            var available = playerLevel >= Mathf.Max(1, definition.requiredLevel) &&
                            factionReputation >= definition.requiredReputation;
            var runtime = new ActivityRuntime(available ? ActivityState.Available : ActivityState.Locked);
            runtimes[definition.activityId] = runtime;
            return runtime;
        }

        public ActivityState GetState(string id) =>
            runtimes.TryGetValue(id, out var runtime) ? runtime.State : ActivityState.Locked;

        public bool Start(string id) => Transition(id, r => r.Start());
        public bool Complete(string id) => Transition(id, r => r.Complete());
        public bool Fail(string id) => Transition(id, r => r.Fail());

        bool Transition(string id, Func<ActivityRuntime, bool> transition)
        {
            if (string.IsNullOrWhiteSpace(id) || !runtimes.TryGetValue(id, out var runtime)) return false;
            if (!transition(runtime)) return false;
            StateChanged?.Invoke(id, runtime.State);
            return true;
        }
    }
}