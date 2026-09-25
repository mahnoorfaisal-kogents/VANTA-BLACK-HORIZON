using NUnit.Framework;
using UnityEngine;
using Vanta.AI;
using Vanta.Vehicles;

public sealed class PolicePursuitCoordinatorVehicleDetectionTests
{
    [Test]
    public void TargetNestedUnderVehicleIsRecognizedAsInVehicle()
    {
        var vehicle = new GameObject("Vehicle");
        vehicle.AddComponent<Rigidbody>();
        vehicle.AddComponent<VehicleController>();

        var seat = new GameObject("DriverSeat");
        seat.transform.SetParent(vehicle.transform);
        var target = new GameObject("Player");
        target.transform.SetParent(seat.transform);

        Assert.IsTrue(PolicePursuitCoordinator.IsTargetInVehicle(target.transform));

        Object.DestroyImmediate(target);
        Object.DestroyImmediate(seat);
        Object.DestroyImmediate(vehicle);
    }

    [Test]
    public void TargetWithoutVehicleAncestorIsNotRecognizedAsInVehicle()
    {
        var target = new GameObject("Player");

        Assert.IsFalse(PolicePursuitCoordinator.IsTargetInVehicle(target.transform));

        Object.DestroyImmediate(target);
    }
}
