using NUnit.Framework;
using UnityEngine;
using Vanta.Core;
using Vanta.Vehicles;

public sealed class VehicleDamageableIntegrationTests
{
    [Test]
    public void VehicleImplementsDamageableContract()
    {
        var go = new GameObject("VehicleDamageableTest");
        go.AddComponent<Rigidbody>();
        var vehicle = go.AddComponent<VehicleController>();
        var damageable = vehicle as IDamageable;

        Assert.IsNotNull(damageable);
        Assert.IsTrue(damageable.IsAlive);
        damageable.ApplyDamage(25f, Vector3.zero, null);
        Assert.AreEqual(75f, vehicle.Health, 0.001f);

        Object.DestroyImmediate(go);
    }
}
