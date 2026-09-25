using UnityEngine;

namespace Vanta.Vehicles
{
    public sealed class TrafficSlotCoordinator : MonoBehaviour
    {
        [SerializeField, Min(1)] private int slotCount = 6;
        private TrafficSlotPlanner planner;

        void Awake() => planner = new TrafficSlotPlanner(slotCount);

        public bool TryReserve(int slot) => planner.TryReserve(slot);
        public void Release(int slot) => planner.Release(slot);
        public bool IsReserved(int slot) => planner.IsReserved(slot);
    }
}
