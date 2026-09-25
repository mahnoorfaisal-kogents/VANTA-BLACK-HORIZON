using NUnit.Framework;
using Vanta.Systems;

public sealed class WorldInteractionConsequenceModelTests
{
    [Test]
    public void DisablingTrafficLightCreatesTrafficDisruption()
    {
        var model = new WorldInteractionConsequenceModel();
        var result = model.Resolve(WorldInteractionDeviceType.TrafficLight, WorldInteractionAction.Disable);

        Assert.IsTrue(result.Applied);
        Assert.Greater(result.Disruption, 0f);
        Assert.Greater(result.PursuitPressure, 0f);
    }

    [Test]
    public void CameraDisableReducesDetectionPressure()
    {
        var model = new WorldInteractionConsequenceModel();
        var result = model.Resolve(WorldInteractionDeviceType.Camera, WorldInteractionAction.Disable);

        Assert.IsTrue(result.Applied);
        Assert.Less(result.PursuitPressure, 0.5f);
    }

    [Test]
    public void UnknownActionDoesNotCreateBenefit()
    {
        var model = new WorldInteractionConsequenceModel();
        var result = model.Resolve(WorldInteractionDeviceType.Alarm, WorldInteractionAction.Enable);

        Assert.IsTrue(result.Applied);
        Assert.GreaterOrEqual(result.PursuitPressure, 0f);
    }
}
