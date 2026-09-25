using NUnit.Framework;
using Vanta.Combat;

public sealed class CombatHealthModelTests
{
    [Test]
    public void DamageReducesHealthAndDestroysAtZero()
    {
        var model = new CombatHealthModel(100f);
        Assert.IsTrue(model.ApplyDamage(35f));
        Assert.AreEqual(65f, model.Health, 0.001f);
        Assert.IsFalse(model.IsDead);
        Assert.IsTrue(model.ApplyDamage(65f));
        Assert.AreEqual(0f, model.Health, 0.001f);
        Assert.IsTrue(model.IsDead);
    }

    [Test]
    public void DeadTargetCannotTakeAdditionalDamage()
    {
        var model = new CombatHealthModel(25f);
        Assert.IsTrue(model.ApplyDamage(25f));
        Assert.IsFalse(model.ApplyDamage(10f));
        Assert.AreEqual(0f, model.Health, 0.001f);
    }

    [Test]
    public void DamageAndHealthAreBounded()
    {
        var model = new CombatHealthModel(100f);
        Assert.IsFalse(model.ApplyDamage(0f));
        Assert.IsFalse(model.ApplyDamage(-10f));
        Assert.AreEqual(100f, model.Health, 0.001f);
    }
}
