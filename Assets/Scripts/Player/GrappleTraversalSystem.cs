using UnityEngine;

namespace Vanta.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class GrappleTraversalSystem : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 25f;
        [SerializeField] private float pullSpeed = 12f;
        [SerializeField] private float stopDistance = 1.5f;
        [SerializeField] private LayerMask anchorMask = ~0;
        [SerializeField] private KeyCode grappleKey = KeyCode.G;

        private CharacterController controller;
        private Vanta.Systems.GrappleTraversalModel model;
        private Transform anchor;

        public bool IsAttached => model != null && model.IsAttached;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            model = new Vanta.Systems.GrappleTraversalModel(maxDistance);
        }

        private void Update()
        {
            if (Input.GetKeyDown(grappleKey))
            {
                if (IsAttached)
                    Detach();
                else
                    TryAttach();
            }

            if (!IsAttached || !anchor)
                return;

            var offset = anchor.position - transform.position;
            var distance = offset.magnitude;

            if (distance <= stopDistance)
            {
                Detach();
                return;
            }

            controller.Move(offset.normalized * pullSpeed * Time.deltaTime);
        }

        private void TryAttach()
        {
            var origin = transform.position + Vector3.up * 1.2f;
            var direction = transform.forward;

            if (!Physics.Raycast(origin, direction, out var hit, maxDistance, anchorMask, QueryTriggerInteraction.Ignore))
                return;

            if (!hit.collider || !hit.collider.CompareTag("GrappleAnchor"))
                return;

            if (!model.TryAttach(hit.distance, hit.collider.GetInstanceID().ToString()))
                return;

            anchor = hit.collider.transform;
        }

        private void Detach()
        {
            model.Detach();
            anchor = null;
        }
    }
}
