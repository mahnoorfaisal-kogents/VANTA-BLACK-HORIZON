using NUnit.Framework;
using Vanta.Racing;

public sealed class RacingActivityRuntimeModelTests
{
    [Test]
    public void CompletionAwardsOnlyOnce()
    {
        var model = new RacingActivityModel(new[] { "a", "b" }, 1, 500);

        Assert.IsTrue(model.Pass("a"));
        Assert.IsTrue(model.Pass("b"));
        Assert.IsTrue(model.IsComplete);
        Assert.AreEqual(500, model.RewardCash);
        Assert.IsFalse(model.Pass("a"));
        Assert.AreEqual(500, model.RewardCash);
    }

    [Test]
    public void InvalidCheckpointOrderDoesNotAdvanceRace()
    {
        var model = new RacingActivityModel(new[] { "a", "b" }, 2, 100);

        Assert.IsFalse(model.Pass("b"));
        Assert.AreEqual(0, model.CompletedLaps);
        Assert.IsTrue(model.Pass("a"));
    }

    [Test]
    public void RewardScalesWithRiskMultiplier()
    {
        var model = new RacingActivityModel(new[] { "a" }, 1, 100);

        Assert.IsTrue(model.Pass("a", 1.5f));
        Assert.AreEqual(150, model.RewardCash);
    }
}
