using NUnit.Framework;
using Vanta.Player;

public sealed class ParkourTraversalModelTests
{
    [Test]
    public void VaultRequiresReachableForwardObstacle()
    {
        var model = new ParkourTraversalModel(1.2f, 1.4f);

        Assert.IsTrue(model.CanVault(1f, 1f));
        Assert.IsFalse(model.CanVault(1.3f, 1f));
        Assert.IsFalse(model.CanVault(1f, 1.5f));
    }

    [Test]
    public void VaultParametersAreClampedToSafeMinimums()
    {
        var model = new ParkourTraversalModel(0f, 0f);

        Assert.IsFalse(model.CanVault(0.1f, 0.1f));
        Assert.Greater(model.VaultForwardDistance, 0f);
        Assert.Greater(model.VaultHeight, 0f);
    }
}
