using NUnit.Framework;
using UnityEngine;
using Vanta.Core;
using Vanta.UI;

public sealed class DeathRestartInputTests
{
    [Test]
    public void EnterMapsToRestartWhenSessionIsDead()
    {
        var sessionObject = new GameObject("Session");
        var session = sessionObject.AddComponent<GameSession>();
        session.StartGame();
        session.MarkDead();

        var pauseObject = new GameObject("Pause");
        var controller = pauseObject.AddComponent<VantaPauseController>();
        controller.Configure(session);

        Assert.IsTrue(controller.HandleRestartInput(KeyCode.Return));
        Assert.AreEqual(GameplayState.Playing, session.State);

        Object.DestroyImmediate(pauseObject);
        Object.DestroyImmediate(sessionObject);
        Time.timeScale = 1f;
    }
}
