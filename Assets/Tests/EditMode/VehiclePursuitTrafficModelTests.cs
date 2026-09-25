using NUnit.Framework;
using Vanta.Vehicles;

public sealed class VehiclePursuitTrafficModelTests
{
    [Test]
    public void PursuitEscalationSelectsRoadblockAtHighHeat()
    {
        var model = new VehiclePursuitTacticsModel();

        Assert.AreEqual(VehiclePursuitTactic.Pursue, model.Resolve(1, false));
        Assert.AreEqual(VehiclePursuitTactic.Intercept, model.Resolve(3, true));
        Assert.AreEqual(VehiclePursuitTactic.Roadblock, model.Resolve(4, true));
        Assert.AreEqual(VehiclePursuitTactic.EscalatedContainment, model.Resolve(5, true));
    }

    [Test]
    public void DamagedPursuitVehicleGetsReducedResponse()
    {
        var model = new VehiclePursuitTacticsModel();

        Assert.AreEqual(1f, model.SpeedMultiplier(100f), 0.001f);
        Assert.Less(model.SpeedMultiplier(40f), 1f);
        Assert.Greater(model.SpeedMultiplier(40f), 0f);
    }

    [Test]
    public void TrafficSlotPlannerAvoidsBlockedAndDuplicateSlots()
    {
        var planner = new TrafficSlotPlanner(3);

        Assert.IsTrue(planner.TryReserve(0));
        Assert.IsFalse(planner.TryReserve(0));
        Assert.IsFalse(planner.TryReserve(3));
        Assert.IsTrue(planner.TryReserve(1));
        Assert.IsTrue(planner.TryReserve(2));
        Assert.IsFalse(planner.TryReserve(2));
    }
}
