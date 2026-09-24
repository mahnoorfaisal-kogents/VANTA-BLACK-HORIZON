using UnityEngine;

namespace Vanta.Combat
{
    [CreateAssetMenu(menuName = "Vanta/Combat/Weapon Data")]
    public sealed class WeaponData : ScriptableObject
    {
        public string weaponId = "sidearm";
        public float damage = 25f;
        public float range = 120f;
        public float roundsPerSecond = 6f;
        public int magazineSize = 12;
        public float reloadSeconds = 1.4f;
        public LayerMask hitMask = ~0;
    }
}
