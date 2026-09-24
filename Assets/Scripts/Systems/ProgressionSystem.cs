using System;
using UnityEngine;

namespace Vanta.Systems
{
    public sealed class ProgressionSystem : MonoBehaviour
    {
        [SerializeField] int xp;
        public int Xp => xp;
        public int Level => Mathf.Max(1, xp / 500 + 1);
        public event Action<int> LevelChanged;
        public void AddXp(int amount)
        {
            if (amount <= 0) return;
            var before=Level; xp=Mathf.Max(0,xp+amount);
            var after=Level; if (after != before) LevelChanged?.Invoke(after);
        }
        public bool MeetsLevel(int required) => Level >= Mathf.Max(1,required);
    }
}