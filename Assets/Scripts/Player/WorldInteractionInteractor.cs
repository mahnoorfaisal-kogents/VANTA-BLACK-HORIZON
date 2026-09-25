using UnityEngine;
using Vanta.Systems;

namespace Vanta.Player
{
    public sealed class WorldInteractionInteractor : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float interactionRange = 4f;
        [SerializeField] private Camera interactionCamera;
        [SerializeField] private LayerMask interactionMask = ~0;
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        public WorldInteractionDevice CurrentDevice { get; private set; }

        private void Update()
        {
            if (!interactionCamera && Camera.main)
                interactionCamera = Camera.main;

            CurrentDevice = FindDevice();
            if (CurrentDevice && Input.GetKeyDown(interactionKey))
                CurrentDevice.Interact();
        }

        private WorldInteractionDevice FindDevice()
        {
            if (!interactionCamera)
                return null;

            var ray = interactionCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (!Physics.Raycast(ray, out var hit, interactionRange, interactionMask, QueryTriggerInteraction.Ignore))
                return null;

            return hit.collider.GetComponentInParent<WorldInteractionDevice>();
        }
    }
}
