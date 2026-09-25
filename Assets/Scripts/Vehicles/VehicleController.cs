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
        [SerializeField] float baseGrip = 1f;
        [SerializeField, Range(0f, 1f)] float minimumGrip = 0.35f;

        Rigidbody body;
        VehicleDamageState damageState;
        VehicleHandlingModel handlingModel;

        public float Health => damageState?.Health ?? 0f;
        public bool IsDestroyed => damageState?.IsDestroyed ?? true;
        public event Action Destroyed;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            damageState = new VehicleDamageState(maxHealth);
            handlingModel = new VehicleHandlingModel(maxSpeed, baseGrip, minimumGrip);
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
            var healthPercent = maxHealth <= 0f ? 0f : Health / maxHealth * 100f;
            var effectiveSpeed = handlingModel.EffectiveMaxSpeed(healthPercent);
            var effectiveGrip = handlingModel.EffectiveGrip(healthPercent);
            body.AddForce(transform.forward * throttle * acceleration, ForceMode.Acceleration);
            body.MoveRotation(body.rotation * Quaternion.Euler(
                0,
                steer * turnRate * effectiveGrip * Time.fixedDeltaTime * Mathf.Clamp01(body.velocity.magnitude / 5f),
                0));

            if (body.velocity.magnitude > effectiveSpeed)
                body.velocity = body.velocity.normalized * effectiveSpeed;
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
