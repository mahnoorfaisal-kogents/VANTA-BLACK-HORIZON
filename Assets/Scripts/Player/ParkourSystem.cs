using UnityEngine;

namespace Vanta.Player
{
    public sealed class ParkourSystem : MonoBehaviour
    {
        [SerializeField] private float checkDistance = 1.2f;
        [SerializeField] private float vaultHeight = 1.4f;
        [SerializeField] private float vaultSpeed = 5f;

        private CharacterController controller;
        private ParkourTraversalModel model;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            model = new ParkourTraversalModel(checkDistance, vaultHeight);
        }

        private void Update()
        {
            if (!controller || !Input.GetKeyDown(KeyCode.Space))
                return;

            var origin = transform.position + Vector3.up * 0.6f;
            if (!Physics.Raycast(origin, transform.forward, out var hit, model.CheckDistance) ||
                !hit.collider)
                return;

            var obstacleHeight = hit.point.y - transform.position.y;
            if (model.CanVault(hit.distance, obstacleHeight))
                StartCoroutine(Vault());
        }

        private System.Collections.IEnumerator Vault()
        {
            var t = 0f;
            var start = transform.position;
            var end = start + transform.forward * model.VaultForwardDistance + Vector3.up * 0.5f;

            while (t < 1f)
            {
                t += Time.deltaTime * vaultSpeed;
                controller.Move(Vector3.Lerp(start, end, t) - transform.position);
                yield return null;
            }
        }
    }
}
