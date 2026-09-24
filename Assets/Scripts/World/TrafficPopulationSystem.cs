using System.Collections.Generic;
using UnityEngine;
using Vanta.Systems;

namespace Vanta.World
{
    public sealed class TrafficPopulationModel
    {
        readonly HashSet<string> active = new();
        readonly int capacity;
        public int ActiveCount => active.Count;
        public TrafficPopulationModel(int capacity) => this.capacity = Mathf.Max(0, capacity);
        public bool TrySpawn(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId) || active.Contains(vehicleId) || active.Count >= capacity) return false;
            active.Add(vehicleId); return true;
        }
        public bool Despawn(string vehicleId) => !string.IsNullOrWhiteSpace(vehicleId) && active.Remove(vehicleId);
        public bool Contains(string vehicleId) => !string.IsNullOrWhiteSpace(vehicleId) && active.Contains(vehicleId);
    }

    public sealed class TrafficPopulationSystem : MonoBehaviour
    {
        [SerializeField, Min(0)] int capacity = 32;
        [SerializeField] PerformanceBudgetSystem performanceBudget;
        TrafficPopulationModel model;

        void Awake() => model = new TrafficPopulationModel(EffectiveCapacity());
        int EffectiveCapacity() => performanceBudget ? Mathf.Min(capacity, performanceBudget.MaxActiveTraffic) : capacity;

        public bool TrySpawn(string id)
        {
            model ??= new TrafficPopulationModel(EffectiveCapacity());
            if (performanceBudget && !performanceBudget.CanSpawnTraffic(model.ActiveCount)) return false;
            return model.TrySpawn(id);
        }

        public bool Despawn(string id) => (model ??= new TrafficPopulationModel(EffectiveCapacity())).Despawn(id);
        public int ActiveCount => model?.ActiveCount ?? 0;
    }
}