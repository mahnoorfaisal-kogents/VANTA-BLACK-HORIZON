using NUnit.Framework;
using UnityEngine;
using Vanta.Player;

namespace Vanta.Tests
{
    public sealed class MovementMathTests
    {
        [Test]
        public void ForwardInputFollowsCameraForward()
        {
            var cameraObject = new GameObject("Camera");
            cameraObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            var result = MovementMath.CameraRelative(Vector2.up, cameraObject.transform);

            Assert.That(result.x, Is.EqualTo(1f).Within(0.001f));
            Assert.That(result.z, Is.EqualTo(0f).Within(0.001f));

            Object.DestroyImmediate(cameraObject);
        }

        [Test]
        public void DiagonalInputIsClamped()
        {
            var cameraObject = new GameObject("Camera");
            var result = MovementMath.CameraRelative(Vector2.one, cameraObject.transform);

            Assert.That(result.magnitude, Is.EqualTo(1f).Within(0.001f));

            Object.DestroyImmediate(cameraObject);
        }
    }
}
