using NUnit.Framework;
using Vanta.Missions;

public sealed class MissionApproachModelTests
{
    [Test]
    public void StealthApproachRewardsLowHeatAndHigherReputation()
    {
        var model = new MissionApproachModel();

        var result = model.Resolve(MissionApproach.Stealth);

        Assert.Less(result.WantedHeat, 1f);
        Assert.Greater(result.ReputationDelta, 0);
        Assert.Greater(result.RewardMultiplier, 1f);
    }

    [Test]
    public void AssaultApproachCreatesMoreHeatButLowerTimeCost()
    {
        var model = new MissionApproachModel();

        var result = model.Resolve(MissionApproach.Assault);

        Assert.Greater(result.WantedHeat, 1f);
        Assert.Greater(result.TimePressure, 0f);
    }

    [Test]
    public void VehicleApproachIsBalanced()
    {
        var model = new MissionApproachModel();

        var result = model.Resolve(MissionApproach.Vehicle);

        Assert.Greater(result.RewardMultiplier, 1f);
        Assert.Greater(result.WantedHeat, 0f);
    }
}
