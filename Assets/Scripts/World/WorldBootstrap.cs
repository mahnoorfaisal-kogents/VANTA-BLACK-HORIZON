using UnityEngine;
using Vanta.CameraSystem;

namespace Vanta.World
{
    public sealed class WorldBootstrap : MonoBehaviour
    {
        [SerializeField] private ThirdPersonCamera cameraRig;
        [SerializeField] private Transform player;
        private void Start()
        {
            if (cameraRig && player) cameraRig.SetTarget(player);
        }
    }
}
