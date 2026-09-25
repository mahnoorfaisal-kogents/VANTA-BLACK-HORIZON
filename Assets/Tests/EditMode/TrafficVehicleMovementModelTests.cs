using NUnit.Framework;
using UnityEngine;
using Vanta.World;

public sealed class TrafficVehicleMovementModelTests
{
    [Test]
    public void MovesTowardNextRoutePointAtConfiguredSpeed()
    {
        var model = new TrafficVehicleMovementModel();
        var next = model.NextPosition(
            Vector3.zero,
            new Vector3(0f, 0f, 10f),
            5f,
            1f);

        Assert.AreEqual(new Vector3(0f, 0f, 5f), next);
    }

    [Test]
    public void SnapsToTargetWhenWithinTravelDistance()
    {
        var model = new TrafficVehicleMovementModel();
        var next = model.NextPosition(
            Vector3.zero,
            new Vector3(0f, 0f, 2f),
            5f,
            1f);

        Assert.AreEqual(new Vector3(0f, 0f, 2f), next);
    }
}
