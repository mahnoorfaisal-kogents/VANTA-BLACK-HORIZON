using System;
using UnityEngine;

namespace Vanta.Combat
{
    public sealed class DamageReceiver : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        private CombatHealthModel model;

        public float Health => model?.Health ?? maxHealth;
        public float MaxHealth => model?.MaxHealth ?? Mathf.Max(1f, maxHealth);
        public bool IsDead => model?.IsDead ?? false;

        public event Action<float> Damaged;
        public event Action Died;

        private void Awake() => Configure(maxHealth);

        public void Configure(float health)
        {
            model = new CombatHealthModel(health);
        }

        public bool ApplyDamage(float amount)
        {
            if (model == null)
                Configure(maxHealth);

            if (!model.ApplyDamage(amount))
                return false;

            Damaged?.Invoke(amount);
            if (model.IsDead)
                Died?.Invoke();
            return true;
        }

        public void RestoreFull()
        {
            if (model == null)
                Configure(maxHealth);
            model.RestoreFull();
        }
    }
}
