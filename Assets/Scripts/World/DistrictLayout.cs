using UnityEngine;

namespace Vanta.World
{
    public static class DistrictLayout
    {
        public static readonly Vector3 PlayerSpawn = new Vector3(0f, 1.1f, -14f);
        public static readonly Vector3 EnemySpawn = new Vector3(0f, 1f, 12f);
        public static readonly Vector3 SafehouseSpawn = new Vector3(-18f, 1f, -12f);
        public static readonly Vector3 GarageSpawn = new Vector3(18f, 1f, -12f);

        public const float DistrictSize = 80f;
        public const int BuildingCount = 16;
        public const int CivilianCount = 6;
        public const int VehicleCount = 4;

        public static Vector3 BuildingPosition(int index)
        {
            var x = -30f + (index % 4) * 20f;
            var z = 18f + (index / 4) * 16f;
            return new Vector3(x, 4f, z);
        }
    }
}
