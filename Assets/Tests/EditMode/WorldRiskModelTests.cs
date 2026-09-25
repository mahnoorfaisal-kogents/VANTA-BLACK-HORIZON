using NUnit.Framework;
using Vanta.World;

public sealed class WorldRiskModelTests
{
    [Test]
    public void NightRaisesActivityRiskAndRewardMultiplier()
    {
        var model = new WorldRiskModel();

        var day = model.Evaluate(false, 0f);
        var night = model.Evaluate(true, 0f);

        Assert.Greater(night.RiskMultiplier, day.RiskMultiplier);
        Assert.Greater(night.RewardMultiplier, day.RewardMultiplier);
    }

    [Test]
    public void ExistingHeatFurtherRaisesRisk()
    {
        var model = new WorldRiskModel();

        var low = model.Evaluate(true, 1f);
        var high = model.Evaluate(true, 4f);

        Assert.Greater(high.RiskMultiplier, low.RiskMultiplier);
        Assert.GreaterOrEqual(high.PursuitPressure, low.PursuitPressure);
    }

    [Test]
    public void HeatIsClamped()
    {
        var model = new WorldRiskModel();

        var result = model.Evaluate(true, 99f);

        Assert.LessOrEqual(result.RiskMultiplier, 2f);
        Assert.LessOrEqual(result.PursuitPressure, 1f);
    }
}
