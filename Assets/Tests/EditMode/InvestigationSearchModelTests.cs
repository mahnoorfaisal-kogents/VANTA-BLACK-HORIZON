using NUnit.Framework;
using Vanta.AI;

public sealed class InvestigationSearchModelTests
{
    [Test]
    public void SearchCyclesThroughUniquePoints()
    {
        var model = new InvestigationSearchModel(new[] { "last_known", "cover_left", "cover_right" });

        Assert.AreEqual("last_known", model.NextPoint());
        Assert.AreEqual("cover_left", model.NextPoint());
        Assert.AreEqual("cover_right", model.NextPoint());
        Assert.AreEqual("last_known", model.NextPoint());
    }

    [Test]
    public void EmptySearchPatternIsRejected()
    {
        Assert.Throws<System.ArgumentException>(() => new InvestigationSearchModel(new string[0]));
    }

    [Test]
    public void ResetRestartsSearch()
    {
        var model = new InvestigationSearchModel(new[] { "a", "b" });

        model.NextPoint();
        model.NextPoint();
        model.Reset();

        Assert.AreEqual("a", model.NextPoint());
    }
}
