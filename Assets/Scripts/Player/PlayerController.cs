using UnityEngine;

namespace Vanta.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float sprintSpeed = 7.5f;
        [SerializeField] private float crouchSpeed = 2.2f;
        [SerializeField] private float jumpHeight = 1.25f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float sprintDrainPerSecond = 18f;
        [SerializeField] private float staminaRecoveryPerSecond = 12f;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float crouchHeight = 1.2f;
        [SerializeField] private float crouchTransitionSpeed = 8f;
        [SerializeField] private Transform movementCamera;

        private CharacterController controller;
        private float verticalVelocity;
        private float standingHeight;
        private Vector3 standingCenter;

        public float Stamina { get; private set; }
        public bool IsCrouching { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            standingHeight = controller.height;
            standingCenter = controller.center;
            Stamina = maxStamina;
        }

        private void Update()
        {
            if (!movementCamera && Camera.main)
                movementCamera = Camera.main.transform;

            IsCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
            var input = Vector2.ClampMagnitude(
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);

            var sprinting = Input.GetKey(KeyCode.LeftShift) &&
                            input.y > 0.1f &&
                            !IsCrouching &&
                            Stamina > 0f;

            var speed = IsCrouching ? crouchSpeed : sprinting ? sprintSpeed : walkSpeed;

            if (sprinting)
                Stamina = Mathf.Max(0f, Stamina - sprintDrainPerSecond * Time.deltaTime);
            else
                Stamina = Mathf.Min(maxStamina, Stamina + staminaRecoveryPerSecond * Time.deltaTime);

            var direction = MovementMath.CameraRelative(input, movementCamera);
            var move = direction * speed;

            if (direction.sqrMagnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    1f - Mathf.Exp(-12f * Time.deltaTime));
            }

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space) && !IsCrouching)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity += gravity * Time.deltaTime;
            move.y = verticalVelocity;
            controller.Move(move * Time.deltaTime);

            var targetHeight = IsCrouching ? crouchHeight : standingHeight;
            controller.height = Mathf.MoveTowards(
                controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

            var heightDelta = standingHeight - controller.height;
            controller.center = standingCenter + Vector3.down * (heightDelta * 0.5f);
        }
    }
}
