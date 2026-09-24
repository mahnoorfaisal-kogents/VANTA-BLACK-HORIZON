using UnityEngine;

namespace Vanta.Combat
{
    public enum WeaponType { Pistol, SMG, AssaultRifle, Shotgun, PrecisionRifle }

    [CreateAssetMenu(menuName = "Vanta/Combat/Weapon Data")]
    public sealed class WeaponData : ScriptableObject
    {
        public string weaponId = "sidearm";
        public WeaponType weaponType = WeaponType.Pistol;
        [Min(0.1f)] public float damage = 25f;
        [Min(1f)] public float range = 120f;
        [Min(0.1f)] public float roundsPerSecond = 6f;
        [Min(1)] public int magazineSize = 12;
        [Min(0f)] public float reloadSeconds = 1.4f;
        [Range(0f, 10f)] public float spreadDegrees = 0.5f;
        [Min(1)] public int pellets = 1;
        public LayerMask hitMask = ~0;
    }
}
