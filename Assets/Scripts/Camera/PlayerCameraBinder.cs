using UnityEngine;
using Vanta.Player;

namespace Vanta.CameraSystem
{
    public sealed class PlayerCameraBinder : MonoBehaviour
    {
        [SerializeField] private ThirdPersonCamera cameraController;
        [SerializeField] private Transform player;

        private void Awake()
        {
            if (!cameraController)
                cameraController = GetComponent<ThirdPersonCamera>();

            if (!player)
            {
                var playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject)
                    player = playerObject.transform;
            }

            if (!player && FindObjectOfType<PlayerMarker>() is PlayerMarker marker)
                player = marker.transform;
        }

        private void Start()
        {
            if (cameraController && player)
                cameraController.SetTarget(player);
        }
    }

    public sealed class PlayerMarker : MonoBehaviour { }
}
