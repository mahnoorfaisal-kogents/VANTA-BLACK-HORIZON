using UnityEngine;

namespace Vanta.CameraSystem
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 followOffset = new Vector3(0f, 2.2f, -4.5f);
        [SerializeField] private float sensitivity = 2.5f;
        [SerializeField] private float followSharpness = 12f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float collisionRadius = 0.2f;
        [SerializeField] private float minDistance = 0.8f;
        [SerializeField] private float aimDistance = 2.6f;
        [SerializeField] private float normalFov = 60f;
        [SerializeField] private float aimFov = 42f;
        [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;

        private float yaw;
        private float pitch = 10f;
        private Camera cachedCamera;

        public bool IsAiming { get; private set; }
        public void SetTarget(Transform value) => target = value;

        private void Awake()
        {
            cachedCamera = GetComponent<Camera>();
            if (!cachedCamera) cachedCamera = Camera.main;
            if (cachedCamera) cachedCamera.fieldOfView = normalFov;
        }

        private void LateUpdate()
        {
            if (!target) return;

            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity, minPitch, maxPitch);
            IsAiming = Input.GetKey(aimKey);

            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var distance = IsAiming ? aimDistance : Mathf.Abs(followOffset.z);
            var pivot = target.position + Vector3.up * followOffset.y;
            var desiredDirection = rotation * Vector3.back;
            var desiredDistance = distance;

            if (Physics.SphereCast(pivot, collisionRadius, desiredDirection, out var hit,
                distance, ~0, QueryTriggerInteraction.Ignore))
            {
                desiredDistance = Mathf.Clamp(hit.distance - collisionRadius, minDistance, distance);
            }

            var desired = pivot + desiredDirection * desiredDistance;
            transform.position = Vector3.Lerp(transform.position, desired,
                1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation,
                1f - Mathf.Exp(-followSharpness * Time.deltaTime));

            if (cachedCamera)
            {
                var targetFov = IsAiming ? aimFov : normalFov;
                cachedCamera.fieldOfView = Mathf.Lerp(cachedCamera.fieldOfView, targetFov,
                    1f - Mathf.Exp(-12f * Time.deltaTime));
            }
        }
    }
}
