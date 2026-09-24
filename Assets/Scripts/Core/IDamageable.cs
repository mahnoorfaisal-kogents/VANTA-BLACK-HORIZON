using UnityEngine;

namespace Vanta.Core
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ApplyDamage(float amount, Vector3 hitPoint, GameObject source);
    }
}
