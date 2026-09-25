using UnityEngine;

namespace Vanta.Player
{
    public sealed class ParkourTraversalModel
    {
        public float CheckDistance { get; }
        public float VaultHeight { get; }
        public float VaultForwardDistance { get; }

        public ParkourTraversalModel(float checkDistance, float vaultHeight, float vaultForwardDistance = 1.2f)
        {
            CheckDistance = Mathf.Max(0.1f, checkDistance);
            VaultHeight = Mathf.Max(0.1f, vaultHeight);
            VaultForwardDistance = Mathf.Max(0.25f, vaultForwardDistance);
        }

        public bool CanVault(float obstacleDistance, float obstacleHeight)
        {
            return obstacleDistance >= 0f &&
                   obstacleDistance <= CheckDistance &&
                   obstacleHeight >= 0f &&
                   obstacleHeight <= VaultHeight;
        }
    }
}
