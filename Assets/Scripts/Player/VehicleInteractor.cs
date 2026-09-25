using UnityEngine;
using Vanta.Vehicles;

namespace Vanta.Player
{
    public sealed class VehicleInteractor : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float interactionRange = 3f;
        [SerializeField] private Camera interactionCamera;
        [SerializeField] private LayerMask interactionMask = ~0;
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        private VehicleInteraction occupiedVehicle;

        public VehicleInteraction OccupiedVehicle => occupiedVehicle;

        private void Update()
        {
            if (!interactionCamera && Camera.main)
                interactionCamera = Camera.main;

            if (!Input.GetKeyDown(interactionKey))
                return;

            if (occupiedVehicle)
            {
                occupiedVehicle.Exit(transform);
                occupiedVehicle = null;
                return;
            }

            var vehicle = FindVehicle();
            if (vehicle && vehicle.TryEnter(transform))
                occupiedVehicle = vehicle;
        }

        private VehicleInteraction FindVehicle()
        {
            if (!interactionCamera)
                return null;

            var ray = interactionCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (!Physics.Raycast(ray, out var hit, interactionRange, interactionMask, QueryTriggerInteraction.Ignore))
                return null;

            return hit.collider.GetComponentInParent<VehicleInteraction>();
        }
    }
}
