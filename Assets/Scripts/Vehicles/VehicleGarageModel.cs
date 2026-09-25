using System;
using UnityEngine;

namespace Vanta.Vehicles
{
    public enum VehiclePartType
    {
        Engine,
        Chassis,
        Tires,
        Drivetrain,
        Auxiliary
    }

    public sealed class VehicleGarageModel
    {
        readonly float baseMaxHealth;
        readonly float baseGrip;
        readonly float baseMaxSpeed;

        float engineBonus;
        float chassisBonus;
        float tireBonus;
        float drivetrainBonus;
        float auxiliaryBonus;

        public VehicleGarageModel(float maxHealth, float grip, float maxSpeed)
        {
            if (maxHealth <= 0f) throw new ArgumentOutOfRangeException(nameof(maxHealth));
            if (grip <= 0f) throw new ArgumentOutOfRangeException(nameof(grip));
            if (maxSpeed <= 0f) throw new ArgumentOutOfRangeException(nameof(maxSpeed));
            baseMaxHealth = maxHealth;
            baseGrip = grip;
            baseMaxSpeed = maxSpeed;
        }

        public float EffectiveAcceleration => Mathf.Clamp(12f * (1f + engineBonus + drivetrainBonus * 0.5f), 1f, 18f);
        public float EffectiveGrip => Mathf.Clamp(baseGrip * (1f + tireBonus + chassisBonus * 0.5f), 0.25f, 1.75f);
        public float EffectiveMaxSpeed => Mathf.Clamp(baseMaxSpeed * (1f + engineBonus * 0.35f + drivetrainBonus * 0.2f), baseMaxSpeed * 0.8f, baseMaxSpeed * 1.35f);
        public float EffectiveMaxHealth => baseMaxHealth * (1f + chassisBonus * 0.2f);

        public bool Install(VehiclePartType type, float normalizedUpgrade)
        {
            if (!Enum.IsDefined(typeof(VehiclePartType), type) || normalizedUpgrade < 0f || normalizedUpgrade > 1f)
                return false;

            switch (type)
            {
                case VehiclePartType.Engine: engineBonus = Mathf.Clamp01(normalizedUpgrade); break;
                case VehiclePartType.Chassis: chassisBonus = Mathf.Clamp01(normalizedUpgrade); break;
                case VehiclePartType.Tires: tireBonus = Mathf.Clamp01(normalizedUpgrade); break;
                case VehiclePartType.Drivetrain: drivetrainBonus = Mathf.Clamp01(normalizedUpgrade); break;
                case VehiclePartType.Auxiliary: auxiliaryBonus = Mathf.Clamp01(normalizedUpgrade); break;
            }

            return true;
        }

        public float Repair(float currentHealth, float amount)
        {
            if (amount < 0f) return Mathf.Clamp(currentHealth, 0f, EffectiveMaxHealth);
            return Mathf.Clamp(currentHealth + amount, 0f, EffectiveMaxHealth);
        }
    }
}
