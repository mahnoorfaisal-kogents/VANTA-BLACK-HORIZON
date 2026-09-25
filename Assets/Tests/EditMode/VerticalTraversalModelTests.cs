using NUnit.Framework;
using Vanta.Player;

public sealed class VerticalTraversalModelTests
{
    [Test]
    public void RoutePrefersGrappleWhenAnchorIsReachable()
    {
        var model = new VerticalTraversalModel(25f);

        Assert.AreEqual(TraversalAction.Grapple, model.Resolve(true, 18f, true));
    }

    [Test]
    public void RouteFallsBackToVaultWhenGrappleIsUnavailable()
    {
        var model = new VerticalTraversalModel(25f);

        Assert.AreEqual(TraversalAction.Vault, model.Resolve(false, 0f, true));
    }

    [Test]
    public void RouteRejectsMissingTraversalOpportunity()
    {
        var model = new VerticalTraversalModel(25f);

        Assert.AreEqual(TraversalAction.None, model.Resolve(false, 0f, false));
        Assert.AreEqual(TraversalAction.None, model.Resolve(true, 30f, true));
    }
}
