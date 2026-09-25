namespace Vanta.Combat
{
    public sealed class MeleeCombatModel
    {
        public float Damage { get; }
        public float Cooldown { get; }
        private float nextAttackTime = float.NegativeInfinity;

        public MeleeCombatModel(float damage, float cooldown)
        {
            Damage = damage < 0f ? 0f : damage;
            Cooldown = cooldown < 0f ? 0f : cooldown;
        }

        public bool TryAttack(float now)
        {
            if (now < nextAttackTime)
                return false;

            nextAttackTime = now + Cooldown;
            return true;
        }
    }
}
