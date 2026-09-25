using NUnit.Framework;
using Vanta.Vehicles;

public sealed class VehicleRecoveryModelTests
{
    [Test]
    public void RecoveryIsLimitedByCooldown()
    {
        var model = new VehicleRecoveryModel(100f, 25f, 3f);

        Assert.AreEqual(75f, model.Repair(50f, 10f, 0f), 0.001f);
        Assert.AreEqual(75f, model.Repair(50f, 10f, 1f), 0.001f);
        Assert.AreEqual(85f, model.Repair(50f, 10f, 3f), 0.001f);
    }

    [Test]
    public void RepairNeverExceedsMaximum()
    {
        var model = new VehicleRecoveryModel(100f, 50f, 2f);

        Assert.AreEqual(100f, model.Repair(90f, 20f, 2f), 0.001f);
    }
}
