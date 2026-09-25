using UnityEngine;

namespace Vanta.Systems
{
    public sealed class WorldInteractionCrimeModel
    {
        public float ResolveCrime(float pursuitPressure)
        {
            return Mathf.Clamp(pursuitPressure, 0f, 5f);
        }
    }
}
