using NUnit.Framework;
using Vanta.Combat;

public sealed class MeleeCombatModelTests
{
    [Test]
    public void AttackConsumesCooldownAndAllowsNextAttackAfterInterval()
    {
        var model = new MeleeCombatModel(25f, 1f);
        Assert.IsTrue(model.TryAttack(0f));
        Assert.IsFalse(model.TryAttack(0.5f));
        Assert.IsTrue(model.TryAttack(1f));
    }

    [Test]
    public void InvalidAttackParametersAreBounded()
    {
        var model = new MeleeCombatModel(-5f, 0.1f);
        Assert.AreEqual(0f, model.Damage);
        Assert.IsTrue(model.TryAttack(0f));
        Assert.IsFalse(model.TryAttack(0f));
    }
}
