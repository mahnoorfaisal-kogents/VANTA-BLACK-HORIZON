using System;
using UnityEngine;

namespace Vanta.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleController : MonoBehaviour
    {
        [SerializeField] float acceleration = 12f;
        [SerializeField] float turnRate = 70f;
        [SerializeField] float maxSpeed = 28f;
        [SerializeField] float maxHealth = 100f;

        Rigidbody body;
        VehicleDamageState damageState;

        public float Health => damageState?.Health ?? 0f;
        public bool IsDestroyed => damageState?.IsDestroyed ?? true;
        public event Action Destroyed;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            damageState = new VehicleDamageState(maxHealth);
            damageState.Destroyed += HandleDestroyed;
        }

        void OnDestroy()
        {
            if (damageState != null) damageState.Destroyed -= HandleDestroyed;
        }

        void FixedUpdate()
        {
            if (IsDestroyed) return;

            float throttle = Input.GetAxis("Vertical");
            float steer = Input.GetAxis("Horizontal");
            body.AddForce(transform.forward * throttle * acceleration, ForceMode.Acceleration);
            body.MoveRotation(body.rotation * Quaternion.Euler(
                0,
                steer * turnRate * Time.fixedDeltaTime * Mathf.Clamp01(body.velocity.magnitude / 5f),
                0));

            if (body.velocity.magnitude > maxSpeed)
                body.velocity = body.velocity.normalized * maxSpeed;
        }

        public void ApplyDamage(float amount)
        {
            damageState?.ApplyDamage(amount);
        }

        public void Repair(float amount)
        {
            if (damageState == null) return;
            damageState.Repair(amount);
        }

        void HandleDestroyed()
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            Destroyed?.Invoke();
        }
    }
}
