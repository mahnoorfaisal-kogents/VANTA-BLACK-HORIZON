using NUnit.Framework;
using UnityEngine;
using Vanta.Combat;

public sealed class DamageReceiverTests
{
    [Test]
    public void ReceiverAppliesDamageAndEmitsDeath()
    {
        var go = new GameObject("DamageReceiverTest");
        var receiver = go.AddComponent<DamageReceiver>();
        receiver.Configure(20f);

        var damageEvents = 0;
        var deathEvents = 0;
        receiver.Damaged += _ => damageEvents++;
        receiver.Died += () => deathEvents++;

        Assert.IsTrue(receiver.ApplyDamage(8f));
        Assert.AreEqual(12f, receiver.Health, 0.001f);
        Assert.AreEqual(1, damageEvents);
        Assert.IsTrue(receiver.ApplyDamage(12f));
        Assert.AreEqual(0f, receiver.Health, 0.001f);
        Assert.AreEqual(1, deathEvents);
        Assert.IsFalse(receiver.ApplyDamage(5f));
        UnityEngine.Object.DestroyImmediate(go);
    }
}
