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
        [Header("Garage")]
        [SerializeField, Range(0f, 1f)] float engineUpgrade;
        [SerializeField, Range(0f, 1f)] float chassisUpgrade;
        [SerializeField, Range(0f, 1f)] float tireUpgrade;
        [SerializeField, Range(0f, 1f)] float drivetrainUpgrade;
        [SerializeField, Range(0f, 1f)] float auxiliaryUpgrade;

        Rigidbody body;
        VehicleDamageState damageState;
        VehicleHandlingModel handlingModel;
        VehicleGarageModel garageModel;

        public float Health => damageState?.Health ?? 0f;
        public bool IsDestroyed => damageState?.IsDestroyed ?? true;
        public event Action Destroyed;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            damageState = new VehicleDamageState(maxHealth);
            handlingModel = new VehicleHandlingModel(maxSpeed, baseGrip, minimumGrip);
            garageModel = new VehicleGarageModel(maxHealth, baseGrip, maxSpeed);
            garageModel.Install(VehiclePartType.Engine, engineUpgrade);
            garageModel.Install(VehiclePartType.Chassis, chassisUpgrade);
            garageModel.Install(VehiclePartType.Tires, tireUpgrade);
            garageModel.Install(VehiclePartType.Drivetrain, drivetrainUpgrade);
            garageModel.Install(VehiclePartType.Auxiliary, auxiliaryUpgrade);
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
            var effectiveSpeed = Mathf.Min(handlingModel.EffectiveMaxSpeed(healthPercent), garageModel.EffectiveMaxSpeed);
            var effectiveGrip = Mathf.Max(handlingModel.EffectiveGrip(healthPercent), minimumGrip * 0.5f);
            var effectiveAcceleration = garageModel.EffectiveAcceleration * (acceleration / 12f);

            body.AddForce(transform.forward * throttle * effectiveAcceleration, ForceMode.Acceleration);
            body.MoveRotation(body.rotation * Quaternion.Euler(
                0,
                steer * turnRate * effectiveGrip * Time.fixedDeltaTime * Mathf.Clamp01(body.velocity.magnitude / 5f),
                0));

            if (body.velocity.magnitude > effectiveSpeed)
                body.velocity = body.velocity.normalized * effectiveSpeed;
        }

        public bool InstallPart(VehiclePartType type, float normalizedUpgrade)
        {
            if (garageModel == null || !garageModel.Install(type, normalizedUpgrade))
                return false;

            switch (type)
            {
                case VehiclePartType.Engine: engineUpgrade = normalizedUpgrade; break;
                case VehiclePartType.Chassis: chassisUpgrade = normalizedUpgrade; break;
                case VehiclePartType.Tires: tireUpgrade = normalizedUpgrade; break;
                case VehiclePartType.Drivetrain: drivetrainUpgrade = normalizedUpgrade; break;
                case VehiclePartType.Auxiliary: auxiliaryUpgrade = normalizedUpgrade; break;
            }

            return true;
        }

        public void ApplyDamage(float amount) => damageState?.ApplyDamage(amount);

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
