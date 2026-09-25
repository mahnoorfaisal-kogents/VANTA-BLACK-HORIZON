using UnityEngine;

namespace Vanta.Vehicles
{
    public readonly struct RoadblockPlan
    {
        public readonly bool ShouldDeploy;
        public readonly int Slot;
        public readonly Vector3 Position;
        public readonly Vector3 Forward;

        public RoadblockPlan(bool shouldDeploy, int slot, Vector3 position, Vector3 forward)
        {
            ShouldDeploy = shouldDeploy;
            Slot = slot;
            Position = position;
            Forward = forward;
        }
    }

    public sealed class RoadblockPlanner
    {
        readonly float distanceAhead;
        readonly int slotCount;

        public RoadblockPlanner(float distanceAhead, int slotCount)
        {
            if (distanceAhead < 0f) throw new System.ArgumentOutOfRangeException(nameof(distanceAhead));
            if (slotCount <= 0) throw new System.ArgumentOutOfRangeException(nameof(slotCount));
            this.distanceAhead = distanceAhead;
            this.slotCount = slotCount;
        }

        public RoadblockPlan Plan(VehiclePursuitTactic tactic, Vector3 targetPosition, Vector3 targetForward, int preferredSlot)
        {
            if (tactic != VehiclePursuitTactic.Roadblock &&
                tactic != VehiclePursuitTactic.EscalatedContainment)
                return new RoadblockPlan(false, -1, targetPosition, Vector3.forward);

            var direction = targetForward;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector3.forward;
            else
                direction.Normalize();

            var slot = preferredSlot >= 0 && preferredSlot < slotCount ? preferredSlot : 0;
            var multiplier = tactic == VehiclePursuitTactic.EscalatedContainment ? 1.5f : 1f;
            return new RoadblockPlan(true, slot, targetPosition + direction * distanceAhead * multiplier, direction);
        }
    }

    public sealed class VehiclePursuitMovementModel
    {
        public Vector3 NextPosition(Vector3 current, Vector3 direction, float speed, float healthPercent, float deltaTime)
        {
            if (deltaTime <= 0f || speed <= 0f || direction.sqrMagnitude < 0.0001f)
                return current;

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                return current;

            var normalized = direction.normalized;
            var health = Mathf.Clamp01(healthPercent);
            var healthMultiplier = 0.55f + health * 0.45f;
            return current + normalized * speed * healthMultiplier * deltaTime;
        }
    }
}
