using NUnit.Framework;
using Vanta.Vehicles;

public sealed class VehicleGarageModelTests
{
    [Test]
    public void PerformancePartsImproveVehicleStatsWithinCaps()
    {
        var model = new VehicleGarageModel(100f, 1f, 28f);

        Assert.IsTrue(model.Install(VehiclePartType.Engine, 0.15f));
        Assert.Greater(model.EffectiveAcceleration, 12f);
        Assert.LessOrEqual(model.EffectiveAcceleration, 18f);

        Assert.IsTrue(model.Install(VehiclePartType.Tires, 0.1f));
        Assert.Greater(model.EffectiveGrip, 1f);
    }

    [Test]
    public void InvalidPartChangesAreRejected()
    {
        var model = new VehicleGarageModel(100f, 1f, 28f);

        Assert.IsFalse(model.Install(VehiclePartType.Engine, -1f));
        Assert.IsFalse(model.Install(VehiclePartType.Engine, 2f));
        Assert.IsFalse(model.Install((VehiclePartType)99, 0.1f));
    }

    [Test]
    public void RepairRestoresHealthButNeverExceedsMaximum()
    {
        var model = new VehicleGarageModel(100f, 1f, 28f);

        Assert.AreEqual(75f, model.Repair(75f, 25f), 0.001f);
        Assert.AreEqual(100f, model.Repair(100f, 80f), 0.001f);
    }
}
