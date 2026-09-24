using UnityEngine;

namespace Vanta.Player
{
    public static class MovementMath
    {
        public static Vector3 CameraRelative(Vector2 input, Transform cameraTransform)
        {
            if (cameraTransform == null) return new Vector3(input.x, 0f, input.y);

            var forward = cameraTransform.forward;
            var right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return Vector3.ClampMagnitude(right * input.x + forward * input.y, 1f);
        }
    }
}
