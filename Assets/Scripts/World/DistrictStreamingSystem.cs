using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.World
{
    public enum DistrictStreamState { Unloaded, Loading, Loaded, Unloading }

    public sealed class DistrictStreamingSystem : MonoBehaviour
    {
        readonly Dictionary<string, DistrictStreamState> states = new();
        public event Action<string, DistrictStreamState> StateChanged;

        public DistrictStreamState GetState(string districtId) =>
            states.TryGetValue(districtId, out var state) ? state : DistrictStreamState.Unloaded;

        public bool BeginLoad(string districtId)
        {
            if (string.IsNullOrWhiteSpace(districtId) || GetState(districtId) != DistrictStreamState.Unloaded) return false;
            return Set(districtId, DistrictStreamState.Loading);
        }

        public bool CompleteLoad(string districtId)
        {
            if (GetState(districtId) != DistrictStreamState.Loading) return false;
            return Set(districtId, DistrictStreamState.Loaded);
        }

        public bool BeginUnload(string districtId)
        {
            if (GetState(districtId) != DistrictStreamState.Loaded) return false;
            return Set(districtId, DistrictStreamState.Unloading);
        }

        public bool CompleteUnload(string districtId)
        {
            if (GetState(districtId) != DistrictStreamState.Unloading) return false;
            return Set(districtId, DistrictStreamState.Unloaded);
        }

        bool Set(string id, DistrictStreamState state)
        {
            states[id] = state;
            StateChanged?.Invoke(id, state);
            return true;
        }
    }
}