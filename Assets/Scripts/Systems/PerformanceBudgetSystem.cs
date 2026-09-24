using System;
using UnityEngine;

namespace Vanta.Systems
{
    /// <summary>
    /// Central lightweight performance policy for the vertical slice.
    /// It applies the configured frame-rate target and exposes deterministic
    /// population-budget checks so spawning systems can remain bounded.
    /// </summary>
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

        public event Action<string> BudgetWarning;

        void OnEnable()
        {
            ApplyFrameRate();
        }

        void OnValidate()
        {
            targetFrameRate = Mathf.Max(1, targetFrameRate);
            maxActiveTraffic = Mathf.Max(1, maxActiveTraffic);
            maxActiveCivilians = Mathf.Max(1, maxActiveCivilians);
            if (Application.isPlaying) ApplyFrameRate();
        }

        public void ApplyFrameRate()
        {
            if (!applyFrameRateTarget) return;
            Application.targetFrameRate = Mathf.Max(1, targetFrameRate);
        }

        public bool CanSpawnTraffic(int activeTraffic)
        {
            return activeTraffic >= 0 && activeTraffic < maxActiveTraffic;
        }

        public bool CanSpawnCivilian(int activeCivilians)
        {
            return activeCivilians >= 0 && activeCivilians < maxActiveCivilians;
        }

        public bool IsWithinBudget(int activeTraffic, int activeCivilians)
        {
            return activeTraffic >= 0 && activeCivilians >= 0 &&
                   activeTraffic <= maxActiveTraffic &&
                   activeCivilians <= maxActiveCivilians;
        }

        public void Report(int activeTraffic, int activeCivilians)
        {
            if (activeTraffic > maxActiveTraffic) BudgetWarning?.Invoke("traffic");
            if (activeCivilians > maxActiveCivilians) BudgetWarning?.Invoke("civilians");
        }
    }
}