using System;

namespace Vanta.Vehicles
{
    public sealed class VehicleDamageState
    {
        readonly float maxHealth;
        public float Health { get; private set; }
        public bool IsDestroyed => Health <= 0f;
        public event Action Destroyed;

        public VehicleDamageState(float maxHealth)
        {
            this.maxHealth = Math.Max(0f, maxHealth);
            Health = this.maxHealth;
        }

        public float ApplyDamage(float amount)
        {
            if (IsDestroyed) return 0f;
            Health = Math.Max(0f, Health - Math.Max(0f, amount));
            if (Health <= 0f) Destroyed?.Invoke();
            return Health;
        }

        public void Repair(float amount)
        {
            if (IsDestroyed && maxHealth > 0f && amount <= 0f) return;
            Health = Math.Min(maxHealth, Health + Math.Max(0f, amount));
        }
    }
}
