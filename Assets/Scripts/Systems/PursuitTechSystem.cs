using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Systems
{
    public enum PursuitTech
    {
        SpikeStrip,
        EmpPulse,
        RoadBlock,
        PursuitRam
    }

    public sealed class PursuitTechSystem : MonoBehaviour
    {
        private readonly HashSet<PursuitTech> unlocked = new();

        public bool IsUnlocked(PursuitTech tech) => unlocked.Contains(tech);

        public bool Unlock(PursuitTech tech) => unlocked.Add(tech);

        public bool TryUse(PursuitTech tech)
        {
            return unlocked.Contains(tech);
        }
    }
}
