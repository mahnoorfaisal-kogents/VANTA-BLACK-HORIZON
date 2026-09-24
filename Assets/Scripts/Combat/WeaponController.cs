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
        public int AmmoInMagazine { get; private set; }
        private float nextShotTime;
        private bool reloading;

        private void Awake() { if (weapon) AmmoInMagazine = weapon.magazineSize; }
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
            AmmoInMagazine--;
            var cam = aimCamera ? aimCamera : Camera.main;
            if (!cam) return;
            var ray = cam.ViewportPointToRay(new Vector3(.5f,.5f,0));
            if (Physics.Raycast(ray, out var hit, weapon.range, weapon.hitMask, QueryTriggerInteraction.Ignore))
            {
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.ApplyDamage(weapon.damage, hit.point, gameObject);
            }
        }
        private IEnumerator Reload()
        {
            if (reloading || !weapon || AmmoInMagazine >= weapon.magazineSize) yield break;
            reloading = true; yield return new WaitForSeconds(weapon.reloadSeconds);
            AmmoInMagazine = weapon.magazineSize; reloading = false;
        }
    }
}
