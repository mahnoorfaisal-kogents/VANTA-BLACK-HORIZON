using UnityEngine;
using Vanta.Player;

namespace Vanta.Vehicles
{
    public sealed class VehicleInteraction : MonoBehaviour
    {
        [SerializeField, Min(0.5f)] private float enterRange = 3f;
        [SerializeField] private Transform seat;

        public bool IsOccupied { get; private set; }

        public bool TryEnter(Transform player)
        {
            if (IsOccupied || !player || Vector3.Distance(transform.position, player.position) > enterRange)
                return false;

            IsOccupied = true;
            var controller = player.GetComponent<PlayerController>();
            if (controller)
                controller.enabled = false;

            player.SetParent(seat ? seat : transform);
            player.localPosition = Vector3.zero;
            player.localRotation = Quaternion.identity;
            return true;
        }

        public void Exit(Transform player)
        {
            if (!IsOccupied || !player)
                return;

            player.SetParent(null);
            player.position = transform.position + transform.right * 2f;

            var controller = player.GetComponent<PlayerController>();
            if (controller)
                controller.enabled = true;

            IsOccupied = false;
        }
    }
}
