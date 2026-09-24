namespace Vanta.Combat
{
    public sealed class WeaponAmmoState
    {
        public int Capacity { get; }
        public int Remaining { get; private set; }

        public WeaponAmmoState(int capacity)
        {
            Capacity = capacity < 0 ? 0 : capacity;
            Remaining = Capacity;
        }

        public bool TryConsumeRound()
        {
            if (Remaining <= 0) return false;
            Remaining--;
            return true;
        }

        public void Reload() => Remaining = Capacity;
    }
}
