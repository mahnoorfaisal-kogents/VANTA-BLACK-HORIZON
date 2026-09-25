using NUnit.Framework;
using Vanta.Combat;

public sealed class WeaponModelTests
{
    [Test]
    public void FireConsumesAmmoAndRespectsCooldown()
    {
        var model = new WeaponModel(3, 20f, 0.5f);
        Assert.IsTrue(model.TryFire(0f));
        Assert.AreEqual(2, model.Ammo);
        Assert.IsFalse(model.TryFire(0.25f));
        Assert.IsTrue(model.TryFire(0.5f));
        Assert.AreEqual(1, model.Ammo);
    }

    [Test]
    public void ReloadRestoresMagazineAndFireFailsWhenEmpty()
    {
        var model = new WeaponModel(2, 20f, 0.1f);
        Assert.IsTrue(model.TryFire(0f));
        Assert.IsTrue(model.TryFire(0.1f));
        Assert.IsFalse(model.TryFire(0.2f));
        Assert.AreEqual(0, model.Ammo);
        model.Reload();
        Assert.AreEqual(2, model.Ammo);
        Assert.IsTrue(model.TryFire(0.3f));
    }
}
