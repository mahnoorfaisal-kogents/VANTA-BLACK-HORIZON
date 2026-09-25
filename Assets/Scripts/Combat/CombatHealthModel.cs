using UnityEngine;

namespace Vanta.Combat
{
    public sealed class CombatHealthModel
    {
        public float MaxHealth { get; }
        public float Health { get; private set; }
        public bool IsDead => Health <= 0f;

        public CombatHealthModel(float maxHealth)
        {
            MaxHealth = Mathf.Max(1f, maxHealth);
            Health = MaxHealth;
        }

        public bool ApplyDamage(float amount)
        {
            if (IsDead || amount <= 0f)
                return false;

            Health = Mathf.Max(0f, Health - amount);
            return true;
        }

        public void RestoreFull() => Health = MaxHealth;
    }
}
