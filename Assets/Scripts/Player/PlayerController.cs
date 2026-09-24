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
        private CharacterController controller;
        private float verticalVelocity;
        public float Stamina { get; private set; }
        public bool IsCrouching { get; private set; }

        private void Awake() { controller = GetComponent<CharacterController>(); Stamina = maxStamina; }

        private void Update()
        {
            IsCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input = Vector2.ClampMagnitude(input, 1f);
            var sprinting = Input.GetKey(KeyCode.LeftShift) && input.y > 0.1f && !IsCrouching && Stamina > 0f;
            var speed = IsCrouching ? crouchSpeed : sprinting ? sprintSpeed : walkSpeed;
            if (sprinting) Stamina = Mathf.Max(0f, Stamina - sprintDrainPerSecond * Time.deltaTime);
            else Stamina = Mathf.Min(maxStamina, Stamina + staminaRecoveryPerSecond * Time.deltaTime);

            var move = transform.TransformDirection(new Vector3(input.x, 0f, input.y)) * speed;
            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space) && !IsCrouching)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            verticalVelocity += gravity * Time.deltaTime;
            move.y = verticalVelocity;
            controller.Move(move * Time.deltaTime);
        }
    }
}
