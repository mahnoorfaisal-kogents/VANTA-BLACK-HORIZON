using NUnit.Framework;
using UnityEngine;
using Vanta.Systems;

public sealed class WorldInteractionDeviceRuntimeTests
{
    [Test]
    public void InteractTogglesDeviceStateAndEmitsConsequence()
    {
        var go = new GameObject("WorldDeviceTest");
        var device = go.AddComponent<WorldInteractionDevice>();

        var model = new WorldInteractionModel();
        device.Configure("traffic_test", WorldInteractionDeviceType.TrafficLight, true);
        device.Initialize(model);

        WorldInteractionConsequence? emitted = null;
        device.ConsequenceApplied += result => emitted = result;

        Assert.IsTrue(device.Interact());
        Assert.IsFalse(device.IsEnabled);
        Assert.IsTrue(emitted.HasValue);
        Assert.Greater(emitted.Value.Disruption, 0f);

        Object.DestroyImmediate(go);
    }
}
