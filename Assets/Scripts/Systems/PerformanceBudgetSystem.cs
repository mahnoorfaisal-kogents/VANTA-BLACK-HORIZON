using System;
using UnityEngine;

namespace Vanta.Systems
{
    public sealed class PerformanceBudgetSystem : MonoBehaviour
    {
        [SerializeField, Min(1)] int targetFrameRate = 60;
        [SerializeField, Min(1)] int maxActiveTraffic = 32;
        [SerializeField, Min(1)] int maxActiveCivilians = 48;

        public int TargetFrameRate => targetFrameRate;
        public int MaxActiveTraffic => maxActiveTraffic;
        public int MaxActiveCivilians => maxActiveCivilians;

        public bool IsWithinBudget(int activeTraffic, int activeCivilians)
        {
            return activeTraffic >= 0 && activeCivilians >= 0 &&
                   activeTraffic <= maxActiveTraffic &&
                   activeCivilians <= maxActiveCivilians;
        }

        public event Action<string> BudgetWarning;
        public void Report(int activeTraffic, int activeCivilians)
        {
            if (activeTraffic > maxActiveTraffic) BudgetWarning?.Invoke("traffic");
            if (activeCivilians > maxActiveCivilians) BudgetWarning?.Invoke("civilians");
        }
    }
}