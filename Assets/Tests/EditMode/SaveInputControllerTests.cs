using NUnit.Framework;
using UnityEngine;
using Vanta.Save;

public sealed class SaveInputControllerTests
{
    [Test]
    public void F5AndF9MapToSaveAndLoadActions()
    {
        var go = new GameObject("SaveInput");
        var controller = go.AddComponent<VantaSaveController>();
        controller.Configure(null, "slot_01");

        Assert.AreEqual(VantaSaveAction.Save, controller.ResolveAction(KeyCode.F5));
        Assert.AreEqual(VantaSaveAction.Load, controller.ResolveAction(KeyCode.F9));
        Assert.AreEqual(VantaSaveAction.None, controller.ResolveAction(KeyCode.F1));

        Object.DestroyImmediate(go);
    }
}
