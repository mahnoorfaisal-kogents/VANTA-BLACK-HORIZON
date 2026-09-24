using UnityEngine;

namespace Vanta.World
{
    public sealed class DiscoveryPoint : MonoBehaviour
    {
        [SerializeField] private string discoveryId = "district_landmark";
        [SerializeField] private float revealRadius = 8f;

        private void Update()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (!player) return;

            if (Vector3.Distance(transform.position, player.transform.position) <= revealRadius)
            {
                var system = FindFirstObjectByType<DiscoverySystem>();
                if (system) system.Discover(discoveryId);
            }
        }
    }
}
