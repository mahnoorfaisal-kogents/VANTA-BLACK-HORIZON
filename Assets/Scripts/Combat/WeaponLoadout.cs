using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanta.Combat
{
    [Serializable]
    public sealed class WeaponSlot
    {
        public WeaponData weapon;
        public int reserveAmmo = 120;
    }

    public sealed class WeaponLoadout : MonoBehaviour
    {
        [SerializeField] private List<WeaponSlot> slots = new();
        public int ActiveIndex { get; private set; }

        public IReadOnlyList<WeaponSlot> Slots => slots;

        public bool Select(int index)
        {
            if (index < 0 || index >= slots.Count || slots[index].weapon == null) return false;
            ActiveIndex = index;
            return true;
        }

        public WeaponData ActiveWeapon =>
            ActiveIndex >= 0 && ActiveIndex < slots.Count ? slots[ActiveIndex].weapon : null;
    }
}
