using UnityEngine;

namespace Vanta.Vehicles
{
    public sealed class VehicleRecoveryModel
    {
        readonly float maxHealth;
        readonly float repairAmount;
        readonly float cooldown;

        public VehicleRecoveryModel(float maxHealth, float repairAmount, float cooldown)
        {
            this.maxHealth = Mathf.Max(1f, maxHealth);
            this.repairAmount = Mathf.Max(0f, repairAmount);
            this.cooldown = Mathf.Max(0f, cooldown);
        }

        public float Repair(float currentHealth, float now, float lastRepairTime)
        {
            if (now < lastRepairTime + cooldown)
                return Mathf.Clamp(currentHealth, 0f, maxHealth);

            return Mathf.Clamp(currentHealth + repairAmount, 0f, maxHealth);
        }
    }
}
