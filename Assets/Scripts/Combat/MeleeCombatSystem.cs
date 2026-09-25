using UnityEngine;

namespace Vanta.Combat
{
    public sealed class MeleeCombatSystem : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float damage = 35f;
        [SerializeField, Min(0f)] private float cooldown = 0.65f;
        [SerializeField, Min(0.1f)] private float range = 2.2f;
        [SerializeField, Range(0f, 180f)] private float arc = 100f;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private KeyCode attackKey = KeyCode.F;
        [SerializeField] private Transform attackOrigin;

        private MeleeCombatModel model;

        private void Awake()
        {
            model = new MeleeCombatModel(damage, cooldown);
            if (!attackOrigin)
                attackOrigin = transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(attackKey))
                TryAttack();
        }

        public bool TryAttack()
        {
            if (!model.TryAttack(Time.unscaledTime))
                return false;

            var origin = attackOrigin ? attackOrigin.position : transform.position;
            var forward = attackOrigin ? attackOrigin.forward : transform.forward;
            var hits = Physics.OverlapSphere(origin, range, hitMask, QueryTriggerInteraction.Ignore);

            foreach (var hit in hits)
            {
                var toTarget = hit.transform.position - origin;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude <= 0.0001f)
                    continue;

                if (Vector3.Angle(forward, toTarget.normalized) > arc * 0.5f)
                    continue;

                var damageable = hit.GetComponentInParent<Vanta.Core.IDamageable>();
                damageable?.ApplyDamage(damage, hit.ClosestPoint(origin), gameObject);
            }

            return true;
        }
    }
}
