using NUnit.Framework;
using UnityEngine;
using Vanta.Core;
using Vanta.UI;

public sealed class GameplaySessionRuntimeTests
{
    [Test]
    public void SessionStartsInGameplayWhenRequested()
    {
        var go = new GameObject("Session");
        var session = go.AddComponent<GameSession>();

        Assert.IsTrue(session.StartGame());
        Assert.AreEqual(GameplayState.Playing, session.State);

        Object.DestroyImmediate(go);
        Time.timeScale = 1f;
    }

    [Test]
    public void PauseControllerTogglesSessionStateFromEscapeInput()
    {
        var sessionObject = new GameObject("Session");
        var session = sessionObject.AddComponent<GameSession>();
        Assert.IsTrue(session.StartGame());

        var pauseObject = new GameObject("Pause");
        var pause = pauseObject.AddComponent<VantaPauseController>();
        pause.Configure(session);

        Assert.IsTrue(pause.HandlePauseInput(KeyCode.Escape));
        Assert.AreEqual(GameplayState.Paused, session.State);
        Assert.IsTrue(pause.HandlePauseInput(KeyCode.Escape));
        Assert.AreEqual(GameplayState.Playing, session.State);

        Object.DestroyImmediate(pauseObject);
        Object.DestroyImmediate(sessionObject);
        Time.timeScale = 1f;
    }
}
