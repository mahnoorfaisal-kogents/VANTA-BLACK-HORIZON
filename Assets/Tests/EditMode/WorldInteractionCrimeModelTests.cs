using NUnit.Framework;
using Vanta.Systems;

public sealed class WorldInteractionCrimeModelTests
{
    [Test]
    public void ConvertsPursuitPressureToBoundedCrime()
    {
        var model = new WorldInteractionCrimeModel();
        Assert.AreEqual(0.35f, model.ResolveCrime(0.35f), 0.001f);
        Assert.AreEqual(0f, model.ResolveCrime(0f), 0.001f);
        Assert.AreEqual(5f, model.ResolveCrime(9f), 0.001f);
    }
}
