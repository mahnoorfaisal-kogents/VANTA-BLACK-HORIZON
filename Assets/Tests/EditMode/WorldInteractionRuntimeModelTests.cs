using NUnit.Framework;
using Vanta.Systems;

public sealed class WorldInteractionRuntimeModelTests
{
    [Test]
    public void InteractionSupportsEnableDisableAndUnknownDeviceRejection()
    {
        var model = new WorldInteractionModel();
        model.Register("gate", WorldInteractionDeviceType.SecurityGate, true);

        Assert.IsTrue(model.TryApply("gate", WorldInteractionAction.Disable));
        Assert.IsFalse(model.IsEnabled("gate"));
        Assert.IsFalse(model.TryApply("unknown", WorldInteractionAction.Enable));
    }

    [Test]
    public void BulletTimeUsesBoundedScaleAndCooldown()
    {
        var model = new BulletTimeModel(2f, 4f, 0.2f);

        Assert.IsTrue(model.TryActivate(0f));
        Assert.AreEqual(0.2f, model.CurrentTimeScale, 0.001f);
        Assert.IsFalse(model.TryActivate(1f));

        model.Tick(2f, 2f);
        Assert.AreEqual(1f, model.CurrentTimeScale, 0.001f);
        Assert.IsFalse(model.TryActivate(3f));
        Assert.IsTrue(model.TryActivate(6f));
    }
}
