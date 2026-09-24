using System;
using UnityEngine;

namespace Vanta.Systems
{
    /// <summary>Deterministic, allocation-free population policy shared by world spawners.</summary>
    public readonly struct PerformanceBudgetPolicy
    {
        public int TargetFrameRate { get; }
        public int MaxActiveTraffic { get; }
        public int MaxActiveCivilians { get; }

        public PerformanceBudgetPolicy(int maxActiveTraffic, int maxActiveCivilians, int targetFrameRate = 60)
        {
            TargetFrameRate = Mathf.Max(1, targetFrameRate);
            MaxActiveTraffic = Mathf.Max(0, maxActiveTraffic);
            MaxActiveCivilians = Mathf.Max(0, maxActiveCivilians);
        }

        public bool CanSpawnTraffic(int activeTraffic) => activeTraffic >= 0 && activeTraffic < MaxActiveTraffic;
        public bool CanSpawnCivilian(int activeCivilians) => activeCivilians >= 0 && activeCivilians < MaxActiveCivilians;
        public bool IsWithinBudget(int activeTraffic, int activeCivilians) =>
            activeTraffic >= 0 && activeCivilians >= 0 &&
            activeTraffic <= MaxActiveTraffic && activeCivilians <= MaxActiveCivilians;
    }

    public sealed class PerformanceBudgetSystem : MonoBehaviour
    {
        [SerializeField, Min(1)] int targetFrameRate = 60;
        [SerializeField, Min(1)] int maxActiveTraffic = 32;
        [SerializeField, Min(1)] int maxActiveCivilians = 48;
        [SerializeField] bool applyFrameRateTarget = true;

        public int TargetFrameRate => targetFrameRate;
        public int MaxActiveTraffic => maxActiveTraffic;
        public int MaxActiveCivilians => maxActiveCivilians;
        public bool ApplyFrameRateTarget => applyFrameRateTarget;
        public PerformanceBudgetPolicy Policy => new(maxActiveTraffic, maxActiveCivilians, targetFrameRate);
        public event Action<string> BudgetWarning;

        void OnEnable() => ApplyFrameRate();

        void OnValidate()
        {
            targetFrameRate = Mathf.Max(1, targetFrameRate);
            maxActiveTraffic = Mathf.Max(1, maxActiveTraffic);
            maxActiveCivilians = Mathf.Max(1, maxActiveCivilians);
            if (Application.isPlaying) ApplyFrameRate();
        }

        public void ApplyFrameRate()
        {
            if (applyFrameRateTarget) Application.targetFrameRate = Mathf.Max(1, targetFrameRate);
        }

        public bool CanSpawnTraffic(int activeTraffic) => Policy.CanSpawnTraffic(activeTraffic);
        public bool CanSpawnCivilian(int activeCivilians) => Policy.CanSpawnCivilian(activeCivilians);
        public bool IsWithinBudget(int activeTraffic, int activeCivilians) => Policy.IsWithinBudget(activeTraffic, activeCivilians);

        public void Report(int activeTraffic, int activeCivilians)
        {
            if (activeTraffic > maxActiveTraffic) BudgetWarning?.Invoke("traffic");
            if (activeCivilians > maxActiveCivilians) BudgetWarning?.Invoke("civilians");
        }
    }
}