using UnityEngine;

namespace Vanta.World
{
    public sealed class TrafficVehicleMovementModel
    {
        public Vector3 NextPosition(Vector3 current, Vector3 target, float speed, float deltaTime)
        {
            var travel = Mathf.Max(0f, speed) * Mathf.Max(0f, deltaTime);
            return Vector3.MoveTowards(current, target, travel);
        }
    }
}
