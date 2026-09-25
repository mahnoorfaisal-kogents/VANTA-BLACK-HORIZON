using NUnit.Framework;
using UnityEngine;
using Vanta.Core;
using Vanta.Player;

public sealed class GameSessionDeathTests
{
    [Test]
    public void PlayerDeathMovesSessionToDeadState()
    {
        var root = new GameObject("Session");
        var healthObject = new GameObject("PlayerHealth");
        var health = healthObject.AddComponent<Health>();
        var session = root.AddComponent<GameSession>();
        session.Configure(health);
        session.StartGame();

        health.ApplyDamage(1000f, healthObject.transform.position, null);

        Assert.AreEqual(GameplayState.Dead, session.State);

        Object.DestroyImmediate(healthObject);
        Object.DestroyImmediate(root);
        Time.timeScale = 1f;
    }
}
