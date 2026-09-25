using NUnit.Framework;
using Vanta.Vehicles;
using Vanta.Player;

public sealed class VehicleStealthIntegrationTests
{
    [Test]
    public void VehicleHandlingReducesGripAsDamageIncreases()
    {
        var model = new VehicleHandlingModel(maxSpeed: 30f, baseGrip: 1f, minimumGrip: 0.35f);

        Assert.AreEqual(30f, model.EffectiveMaxSpeed(100f), 0.001f);
        Assert.Less(model.EffectiveMaxSpeed(20f), 30f);
        Assert.GreaterOrEqual(model.EffectiveGrip(0f), 0.35f);
        Assert.GreaterOrEqual(model.EffectiveGrip(100f), 0.35f);
    }

    [Test]
    public void VehicleHandlingRejectsInvalidConfiguration()
    {
        Assert.Throws<System.ArgumentOutOfRangeException>(() => new VehicleHandlingModel(0f, 1f, 0.35f));
        Assert.Throws<System.ArgumentOutOfRangeException>(() => new VehicleHandlingModel(30f, 0f, 0.35f));
        Assert.Throws<System.ArgumentOutOfRangeException>(() => new VehicleHandlingModel(30f, 1f, 1.1f));
    }

    [Test]
    public void StealthPerceptionCombinesVisibilityNoiseAndLight()
    {
        var perception = new StealthPerceptionModel();
        var hidden = perception.Evaluate(visibility: 0.2f, noise: 0.1f, lightExposure: 0.1f, crouched: true);
        var exposed = perception.Evaluate(visibility: 1f, noise: 1f, lightExposure: 1f, crouched: false);

        Assert.Less(hidden.Suspicion, exposed.Suspicion);
        Assert.GreaterOrEqual(hidden.Suspicion, 0f);
        Assert.LessOrEqual(exposed.Suspicion, 1f);
        Assert.IsTrue(hidden.IsHidden);
        Assert.IsFalse(exposed.IsHidden);
    }

    [Test]
    public void StealthSuspicionTransitionsFromInvestigateToAlert()
    {
        var state = new StealthSuspicionState(investigateThreshold: 0.35f, alertThreshold: 0.75f);

        Assert.AreEqual(StealthAlertState.Unaware, state.Update(0.2f));
        Assert.AreEqual(StealthAlertState.Investigating, state.Update(0.5f));
        Assert.AreEqual(StealthAlertState.Alerted, state.Update(0.9f));
        Assert.AreEqual(StealthAlertState.Alerted, state.Update(0.1f));
        state.Reset();
        Assert.AreEqual(StealthAlertState.Unaware, state.Current);
    }
}
