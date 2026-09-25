using UnityEngine;

namespace Vanta.Combat
{
    public sealed class HitscanWeaponSystem : MonoBehaviour
    {
        [SerializeField, Min(1)] private int magazineSize = 12;
        [SerializeField, Min(0f)] private float damage = 25f;
        [SerializeField, Min(0f)] private float fireCooldown = 0.18f;
        [SerializeField, Min(1f)] private float range = 100f;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform muzzle;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode reloadKey = KeyCode.R;

        private WeaponModel model;

        public int Ammo => model?.Ammo ?? 0;
        public int MagazineSize => model?.MagazineSize ?? Mathf.Max(1, magazineSize);

        private void Awake()
        {
            model = new WeaponModel(magazineSize, damage, fireCooldown);
        }

        private void Update()
        {
            if (!aimCamera && Camera.main)
                aimCamera = Camera.main;

            if (Input.GetKeyDown(reloadKey))
                model.Reload();

            if (Input.GetKey(fireKey))
                TryFire();
        }

        public bool TryFire()
        {
            if (!aimCamera || !model.TryFire(Time.unscaledTime))
                return false;

            var ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                var receiver = hit.collider.GetComponentInParent<DamageReceiver>();
                if (receiver)
                    receiver.ApplyDamage(model.Damage);
            }

            return true;
        }

        public void Reload() => model.Reload();
    }
}
