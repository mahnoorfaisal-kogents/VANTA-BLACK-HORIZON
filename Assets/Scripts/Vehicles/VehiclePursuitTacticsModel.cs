using System;
using System.Collections.Generic;

namespace Vanta.Vehicles
{
    public enum VehiclePursuitTactic
    {
        Pursue,
        Intercept,
        Roadblock,
        EscalatedContainment
    }

    public sealed class VehiclePursuitTacticsModel
    {
        public VehiclePursuitTactic Resolve(int wantedLevel, bool targetInVehicle)
        {
            if (wantedLevel <= 0) return VehiclePursuitTactic.Pursue;
            if (wantedLevel >= 5 && targetInVehicle) return VehiclePursuitTactic.EscalatedContainment;
            if (wantedLevel >= 4 && targetInVehicle) return VehiclePursuitTactic.Roadblock;
            if (wantedLevel >= 2 && targetInVehicle) return VehiclePursuitTactic.Intercept;
            return VehiclePursuitTactic.Pursue;
        }

        public float SpeedMultiplier(float healthPercent)
        {
            healthPercent = Math.Max(0f, Math.Min(100f, healthPercent));
            return 0.55f + healthPercent * 0.0045f;
        }
    }

    public sealed class TrafficSlotPlanner
    {
        readonly HashSet<int> reserved = new();
        readonly int slotCount;

        public TrafficSlotPlanner(int slotCount)
        {
            if (slotCount <= 0) throw new ArgumentOutOfRangeException(nameof(slotCount));
            this.slotCount = slotCount;
        }

        public bool TryReserve(int slot)
        {
            if (slot < 0 || slot >= slotCount || reserved.Contains(slot))
                return false;
            reserved.Add(slot);
            return true;
        }

        public void Release(int slot) => reserved.Remove(slot);
        public bool IsReserved(int slot) => reserved.Contains(slot);
    }
}
