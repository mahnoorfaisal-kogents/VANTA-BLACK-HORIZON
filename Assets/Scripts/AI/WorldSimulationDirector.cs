using System;
using UnityEngine;

namespace Vanta.AI
{
    /// <summary>Deterministic systemic pressure director for events, police intensity and NPC activity.</summary>
    public sealed class WorldSimulationDirector
    {
        readonly int seed;

        public WorldSimulationDirector(int seed) => this.seed = seed;

        public float CalculatePressure(float wantedHeat, float playerVisibility, float worldInstability)
        {
            var basePressure = wantedHeat * 0.55f + playerVisibility * 0.2f + worldInstability * 0.25f;
            var seedBias = Mathf.Abs(seed % 17) / 1000f;
            return Mathf.Clamp01(basePressure + seedBias);
        }

        public int ChooseEventTier(float pressure)
        {
            pressure = Mathf.Clamp01(pressure);
            if (pressure >= 0.75f) return 3;
            if (pressure >= 0.45f) return 2;
            if (pressure >= 0.2f) return 1;
            return 0;
        }
    }
}