using UnityEngine;
using Vanta.Core;

namespace Vanta.Player
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        [SerializeField, Min(0f)] private float armor = 0f;
        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;
        public bool IsAlive => CurrentHealth > 0f;
        public float Armor => armor;
        public event System.Action<float> HealthChanged;
        public event System.Action Died;

        private void Awake() => CurrentHealth = maxHealth;

        public void ApplyDamage(float amount, Vector3 hitPoint, GameObject source)
        {
            if (!IsAlive || amount <= 0f) return;
            var absorbed = Mathf.Min(armor, amount * 0.5f);
            armor -= absorbed;
            CurrentHealth = Mathf.Max(0f, CurrentHealth - (amount - absorbed));
            HealthChanged?.Invoke(CurrentHealth);
            if (CurrentHealth <= 0f) Died?.Invoke();
        }

        public void Heal(float amount) => CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + Mathf.Max(0f, amount));
        public void ResetHealth() { CurrentHealth = maxHealth; HealthChanged?.Invoke(CurrentHealth); }
    }
}
