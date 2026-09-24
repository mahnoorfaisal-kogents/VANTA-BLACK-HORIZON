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
        private float yaw;
        private float pitch = 10f;

        public void SetTarget(Transform value) => target = value;

        private void LateUpdate()
        {
            if (!target) return;
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity, minPitch, maxPitch);
            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var desired = target.position + rotation * followOffset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
        }
    }
}
