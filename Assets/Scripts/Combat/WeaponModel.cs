namespace Vanta.Combat
{
    public sealed class WeaponModel
    {
        public int MagazineSize { get; }
        public int Ammo { get; private set; }
        public float Damage { get; }
        public float FireCooldown { get; }

        private float nextFireTime = float.NegativeInfinity;

        public WeaponModel(int magazineSize, float damage, float fireCooldown)
        {
            MagazineSize = magazineSize < 1 ? 1 : magazineSize;
            Damage = damage < 0f ? 0f : damage;
            FireCooldown = fireCooldown < 0f ? 0f : fireCooldown;
            Ammo = MagazineSize;
        }

        public bool TryFire(float now)
        {
            if (Ammo <= 0 || now < nextFireTime)
                return false;

            Ammo--;
            nextFireTime = now + FireCooldown;
            return true;
        }

        public void Reload() => Ammo = MagazineSize;
    }
}
