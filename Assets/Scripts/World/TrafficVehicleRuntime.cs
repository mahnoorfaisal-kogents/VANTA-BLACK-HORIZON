using UnityEngine;

namespace Vanta.World
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class TrafficVehicleRuntime : MonoBehaviour
    {
        [SerializeField] private TrafficSystem trafficSystem;
        [SerializeField] private string routeId;
        [SerializeField, Min(0f)] private float speed = 9f;
        [SerializeField, Min(0.1f)] private float waypointRadius = 1.2f;

        private readonly TrafficVehicleMovementModel movement = new();
        private Rigidbody body;
        private TrafficRoute route;
        private int waypointIndex;

        public int WaypointIndex => waypointIndex;
        public bool HasRoute => route != null;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.interpolation = RigidbodyInterpolation.Interpolate;
            ResolveRoute();
        }

        private void FixedUpdate()
        {
            if (!body || route == null || route.points == null || route.points.Length < 2)
                return;

            var target = route.points[waypointIndex];
            if (Vector3.Distance(body.position, target) <= waypointRadius)
            {
                waypointIndex = (waypointIndex + 1) % route.points.Length;
                target = route.points[waypointIndex];
            }

            var next = movement.NextPosition(body.position, target, speed, Time.fixedDeltaTime);
            body.MovePosition(next);

            var direction = target - body.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
                body.MoveRotation(Quaternion.LookRotation(direction.normalized, Vector3.up));
        }

        public void Configure(TrafficSystem system, string id)
        {
            trafficSystem = system;
            routeId = id;
            ResolveRoute();
        }

        private void ResolveRoute()
        {
            route = trafficSystem ? trafficSystem.GetRoute(routeId) : null;
            waypointIndex = 0;
        }
    }
}
