using NUnit.Framework;
using UnityEngine;
using Vanta.World;

namespace Vanta.Tests
{
    public sealed class DistrictLayoutTests
    {
        [Test]
        public void DistrictSizeIsPositive()
        {
            Assert.That(DistrictLayout.DistrictSize, Is.GreaterThan(0f));
        }

        [Test]
        public void PlayerAndEnemySpawnsAreSeparated()
        {
            Assert.That(Vector3.Distance(DistrictLayout.PlayerSpawn, DistrictLayout.EnemySpawn), Is.GreaterThan(10f));
        }

        [Test]
        public void BuildingLayoutProducesStablePositions()
        {
            Assert.That(DistrictLayout.BuildingPosition(0), Is.EqualTo(new Vector3(-30f, 4f, 18f)));
            Assert.That(DistrictLayout.BuildingPosition(15), Is.EqualTo(new Vector3(30f, 4f, 66f)));
        }
    }
}
