using UnityEngine;

namespace Vanta.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PolicePursuitVehicleRuntime : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private TrafficSlotCoordinator trafficSlots;
        [SerializeField] private float pursuitSpeed = 14f;
        [SerializeField] private float roadblockDistance = 12f;
        [SerializeField, Min(1)] private int roadblockSlots = 6;

        private readonly VehiclePursuitMovementModel movement = new();
        private RoadblockPlanner roadblockPlanner;
        private Rigidbody body;
        private int reservedSlot = -1;

        public VehiclePursuitTactic CurrentTactic { get; private set; } = VehiclePursuitTactic.Pursue;
        public RoadblockPlan CurrentRoadblock { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            roadblockPlanner = new RoadblockPlanner(roadblockDistance, roadblockSlots);
        }

        public void SetTarget(Transform newTarget) => target = newTarget;

        public void SetTactic(VehiclePursuitTactic tactic, int preferredSlot = 0)
        {
            if (CurrentTactic == tactic)
                return;

            ReleaseRoadblockSlot();
            CurrentTactic = tactic;

            if (!target)
            {
                CurrentRoadblock = default;
                return;
            }

            CurrentRoadblock = roadblockPlanner.Plan(
                tactic,
                target.position,
                target.forward,
                preferredSlot);

            if (CurrentRoadblock.ShouldDeploy && trafficSlots &&
                trafficSlots.TryReserve(CurrentRoadblock.Slot))
                reservedSlot = CurrentRoadblock.Slot;
            else if (CurrentRoadblock.ShouldDeploy)
                CurrentRoadblock = default;
        }

        private void FixedUpdate()
        {
            if (!target || !body)
                return;

            var destination = CurrentRoadblock.ShouldDeploy
                ? CurrentRoadblock.Position
                : target.position;

            var direction = destination - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.25f)
                return;

            var healthPercent = 1f;
            var vehicle = GetComponent<VehicleController>();
            if (vehicle)
                healthPercent = vehicle.Health / Mathf.Max(1f, vehicle.MaxHealth);

            var currentPosition = body.position;
            var next = movement.NextPosition(
                currentPosition,
                direction,
                pursuitSpeed,
                healthPercent,
                Time.fixedDeltaTime);

            body.MovePosition(next);
            var flat = next - currentPosition;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.0001f)
                body.MoveRotation(Quaternion.LookRotation(flat.normalized, Vector3.up));
        }

        private void OnDisable() => ReleaseRoadblockSlot();

        private void ReleaseRoadblockSlot()
        {
            if (reservedSlot < 0 || !trafficSlots)
                return;

            trafficSlots.Release(reservedSlot);
            reservedSlot = -1;
        }
    }
}
