using System.Collections.Generic;
using UnityEngine;
using Vanta.Systems;

namespace Vanta.World
{
    public sealed class CivilianPopulationModel
    {
        readonly HashSet<string> active = new();
        readonly int capacity;
        public int ActiveCount => active.Count;
        public CivilianPopulationModel(int capacity) => this.capacity = Mathf.Max(0, capacity);
        public bool TrySpawn(string civilianId)
        {
            if (string.IsNullOrWhiteSpace(civilianId) || active.Contains(civilianId) || active.Count >= capacity) return false;
            active.Add(civilianId); return true;
        }
        public bool Despawn(string civilianId) => !string.IsNullOrWhiteSpace(civilianId) && active.Remove(civilianId);
        public bool Contains(string civilianId) => !string.IsNullOrWhiteSpace(civilianId) && active.Contains(civilianId);
    }

    public sealed class CivilianPopulationSystem : MonoBehaviour
    {
        [SerializeField, Min(0)] int capacity = 48;
        [SerializeField] PerformanceBudgetSystem performanceBudget;
        CivilianPopulationModel model;

        void Awake() => model = new CivilianPopulationModel(EffectiveCapacity());
        int EffectiveCapacity() => performanceBudget ? Mathf.Min(capacity, performanceBudget.MaxActiveCivilians) : capacity;

        public bool TrySpawn(string id)
        {
            model ??= new CivilianPopulationModel(EffectiveCapacity());
            if (performanceBudget && !performanceBudget.CanSpawnCivilian(model.ActiveCount)) return false;
            return model.TrySpawn(id);
        }

        public bool Despawn(string id) => (model ??= new CivilianPopulationModel(EffectiveCapacity())).Despawn(id);
        public int ActiveCount => model?.ActiveCount ?? 0;
    }
}