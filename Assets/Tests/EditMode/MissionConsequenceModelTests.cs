using NUnit.Framework;
using Vanta.Missions;

public sealed class MissionConsequenceModelTests
{
    [Test]
    public void CompletingMissionProducesDeterministicRewardsAndHeat()
    {
        var model = new GameplayConsequenceModel();
        model.Apply(new MissionConsequence
        {
            cash = 500,
            xp = 100,
            wantedHeat = 1.5f,
            factionId = "faction_a",
            reputationDelta = 10,
            territoryId = "district_a",
            territoryDelta = 5,
            revealIds = new[] { "safehouse_a" }
        });

        Assert.AreEqual(500, model.Cash);
        Assert.AreEqual(100, model.Xp);
        Assert.AreEqual(1.5f, model.WantedHeat, 0.001f);
        Assert.AreEqual(10, model.GetReputation("faction_a"));
        Assert.AreEqual(5, model.GetTerritoryInfluence("district_a"));
        Assert.IsTrue(model.IsRevealed("safehouse_a"));
    }

    [Test]
    public void NegativeRewardFieldsCannotCreateNegativeCashXpOrHeat()
    {
        var model = new GameplayConsequenceModel();
        model.Apply(new MissionConsequence { cash = -500, xp = -10, wantedHeat = -3f });

        Assert.AreEqual(0, model.Cash);
        Assert.AreEqual(0, model.Xp);
        Assert.AreEqual(0f, model.WantedHeat, 0.001f);
    }
}
