using NUnit.Framework;
using UnityEngine;
using Vanta.Vehicles;

public sealed class RoadblockPlannerTests
{
    [Test]
    public void PlansRoadblockAheadOfTargetAlongTravelDirection()
    {
        var planner = new RoadblockPlanner(12f, 3);
        var result = planner.Plan(
            VehiclePursuitTactic.Roadblock,
            new Vector3(0f, 0f, 10f),
            new Vector3(0f, 0f, 1f),
            0);

        Assert.IsTrue(result.ShouldDeploy);
        Assert.AreEqual(3, result.Slot);
        Assert.AreEqual(new Vector3(0f, 0f, 22f), result.Position);
        Assert.AreEqual(new Vector3(0f, 0f, 1f), result.Forward);
    }

    [Test]
    public void PursueDoesNotRequestRoadblock()
    {
        var planner = new RoadblockPlanner(12f, 3);
        var result = planner.Plan(
            VehiclePursuitTactic.Pursue,
            Vector3.zero,
            Vector3.forward,
            0);

        Assert.IsFalse(result.ShouldDeploy);
        Assert.AreEqual(-1, result.Slot);
    }

    [Test]
    public void ContainmentUsesWiderOffsetAndValidSlot()
    {
        var planner = new RoadblockPlanner(20f, 4);
        var result = planner.Plan(
            VehiclePursuitTactic.EscalatedContainment,
            Vector3.zero,
            Vector3.right,
            2);

        Assert.IsTrue(result.ShouldDeploy);
        Assert.AreEqual(2, result.Slot);
        Assert.AreEqual(new Vector3(20f, 0f, 0f), result.Position);
    }
}

public sealed class VehiclePursuitMovementModelTests
{
    [Test]
    public void DamagedPoliceVehicleMovesAtReducedSpeed()
    {
        var model = new VehiclePursuitMovementModel();
        var next = model.NextPosition(Vector3.zero, Vector3.forward, 10f, 0.5f, 1f);

        Assert.AreEqual(new Vector3(0f, 0f, 5f), next);
    }

    [Test]
    public void ZeroDirectionKeepsCurrentPosition()
    {
        var model = new VehiclePursuitMovementModel();
        Assert.AreEqual(Vector3.one, model.NextPosition(Vector3.one, Vector3.zero, 10f, 1f, 1f));
    }
}


public sealed class PolicePursuitVehicleRuntimeTests
{
    [Test]
    public void ChangingTargetReplansExistingRoadblockTactic()
    {
        var policeObject = new UnityEngine.GameObject("PolicePursuitVehicleTest");
        policeObject.AddComponent<UnityEngine.Rigidbody>();
        var runtime = policeObject.AddComponent<Vanta.Vehicles.PolicePursuitVehicleRuntime>();

        var firstTargetObject = new UnityEngine.GameObject("TargetA");
        firstTargetObject.transform.position = new UnityEngine.Vector3(0f, 0f, 10f);
        firstTargetObject.transform.forward = UnityEngine.Vector3.forward;

        var secondTargetObject = new UnityEngine.GameObject("TargetB");
        secondTargetObject.transform.position = new UnityEngine.Vector3(20f, 0f, 10f);
        secondTargetObject.transform.forward = UnityEngine.Vector3.right;

        runtime.SetTarget(firstTargetObject.transform);
        runtime.SetTactic(Vanta.Vehicles.VehiclePursuitTactic.Roadblock);
        var firstPlan = runtime.CurrentRoadblock;

        runtime.SetTarget(secondTargetObject.transform);
        var secondPlan = runtime.CurrentRoadblock;

        Assert.AreNotEqual(firstPlan.Position, secondPlan.Position);
        Assert.AreEqual(new UnityEngine.Vector3(32f, 0f, 10f), secondPlan.Position);

        UnityEngine.Object.DestroyImmediate(firstTargetObject);
        UnityEngine.Object.DestroyImmediate(secondTargetObject);
        UnityEngine.Object.DestroyImmediate(policeObject);
    }
}
