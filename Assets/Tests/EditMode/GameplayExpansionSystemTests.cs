using NUnit.Framework;
using Vanta.Systems;

public sealed class GameplayExpansionSystemTests
{
    [Test]
    public void WorldInteractionAppliesOnlySupportedStateTransitions()
    {
        var system = new WorldInteractionModel();
        system.Register("traffic_01", WorldInteractionDeviceType.TrafficLight, true);

        Assert.IsTrue(system.TryApply("traffic_01", WorldInteractionAction.Disable));
        Assert.IsFalse(system.IsEnabled("traffic_01"));
        Assert.IsFalse(system.TryApply("missing", WorldInteractionAction.Enable));
    }

    [Test]
    public void BulletTimeHonorsDurationAndCooldown()
    {
        var model = new BulletTimeModel(duration: 2f, cooldown: 5f, timeScale: 0.25f);

        Assert.IsTrue(model.TryActivate(10f));
        Assert.IsTrue(model.IsActive);
        Assert.AreEqual(0.25f, model.CurrentTimeScale, 0.0001f);

        model.Tick(2.1f, 12.1f);
        Assert.IsFalse(model.IsActive);
        Assert.IsFalse(model.TryActivate(13f));
        Assert.IsTrue(model.TryActivate(18f));
    }

    [Test]
    public void GrappleRejectsOutOfRangeAnchorsAndTracksAttachment()
    {
        var grapple = new GrappleTraversalModel(30f);

        Assert.IsFalse(grapple.TryAttach(31f, "roof"));
        Assert.IsTrue(grapple.TryAttach(20f, "roof"));
        Assert.IsTrue(grapple.IsAttached);
        Assert.AreEqual("roof", grapple.AnchorId);

        grapple.Detach();
        Assert.IsFalse(grapple.IsAttached);
    }

    [Test]
    public void RacingModelRequiresOrderedCheckpointsAndCompletesLaps()
    {
        var race = new RacingEventModel(new[] { "cp0", "cp1", "cp2" }, 2);

        Assert.IsFalse(race.TryPassCheckpoint("cp1"));
        Assert.IsTrue(race.TryPassCheckpoint("cp0"));
        Assert.IsTrue(race.TryPassCheckpoint("cp1"));
        Assert.IsTrue(race.TryPassCheckpoint("cp2"));
        Assert.AreEqual(1, race.CompletedLaps);
        Assert.IsFalse(race.IsComplete);

        race.TryPassCheckpoint("cp0");
        race.TryPassCheckpoint("cp1");
        Assert.IsTrue(race.TryPassCheckpoint("cp2"));
        Assert.IsTrue(race.IsComplete);
    }
}
