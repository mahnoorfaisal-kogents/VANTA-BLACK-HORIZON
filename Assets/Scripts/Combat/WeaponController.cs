using System.Collections;
using UnityEngine;
using Vanta.Core;

namespace Vanta.Combat
{
    public sealed class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponData weapon;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform muzzle;
        WeaponAmmoState ammoState;
        public int AmmoInMagazine => ammoState?.Remaining ?? 0;
        public bool IsReloading => reloading;
        private float nextShotTime;
        private bool reloading;

        private void Awake()
        {
            if (weapon) ammoState = new WeaponAmmoState(weapon.magazineSize);
        }

        private void Update()
        {
            if (!weapon || reloading) return;
            if (Input.GetKeyDown(KeyCode.R)) { StartCoroutine(Reload()); return; }
            if (Input.GetButton("Fire1")) TryFire();
        }

        private void TryFire()
        {
            if (Time.time < nextShotTime) return;
            if (AmmoInMagazine <= 0) { StartCoroutine(Reload()); return; }

            nextShotTime = Time.time + 1f / Mathf.Max(0.01f, weapon.roundsPerSecond);
            if (!ammoState.TryConsumeRound()) return;

            var cam = aimCamera ? aimCamera : Camera.main;
            if (!cam) return;

            for (var pellet = 0; pellet < Mathf.Max(1, weapon.pellets); pellet++)
            {
                var spread = Random.insideUnitCircle * Mathf.Tan(weapon.spreadDegrees * Mathf.Deg2Rad);
                var direction = (cam.transform.forward +
                                 cam.transform.right * spread.x +
                                 cam.transform.up * spread.y).normalized;
                var origin = muzzle ? muzzle.position : cam.transform.position;

                if (!Physics.Raycast(origin, direction, out var hit, weapon.range,
                    weapon.hitMask, QueryTriggerInteraction.Ignore))
                    continue;

                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.ApplyDamage(weapon.damage, hit.point, gameObject);
            }
        }

        private IEnumerator Reload()
        {
            if (reloading || !weapon || AmmoInMagazine >= weapon.magazineSize) yield break;
            reloading = true;
            yield return new WaitForSeconds(weapon.reloadSeconds);
            ammoState.Reload();
            reloading = false;
        }
    }
}
