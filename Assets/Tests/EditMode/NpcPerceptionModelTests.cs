using NUnit.Framework;
using Vanta.AI;

public sealed class NpcPerceptionModelTests
{
    [Test]
    public void HiddenLowSuspicionTargetIsNotDetected()
    {
        var model = new NpcPerceptionModel();
        var result = model.Evaluate(distance: 10f, detectionRange: 35f, suspicion: 0.1f, isHidden: true, wantedActive: false);

        Assert.IsFalse(result.Detected);
        Assert.IsFalse(result.Investigating);
        Assert.AreEqual(0f, result.Confidence, 0.001f);
    }

    [Test]
    public void SuspiciousTargetStartsInvestigation()
    {
        var model = new NpcPerceptionModel();
        var result = model.Evaluate(distance: 12f, detectionRange: 35f, suspicion: 0.5f, isHidden: false, wantedActive: false);

        Assert.IsTrue(result.Detected);
        Assert.IsTrue(result.Investigating);
        Assert.IsFalse(result.Alerted);
        Assert.Greater(result.Confidence, 0f);
    }

    [Test]
    public void HighSuspicionOrWantedTargetAlerts()
    {
        var model = new NpcPerceptionModel();

        var suspicious = model.Evaluate(8f, 35f, 0.9f, false, false);
        var wanted = model.Evaluate(8f, 35f, 0.1f, true, true);

        Assert.IsTrue(suspicious.Alerted);
        Assert.IsTrue(wanted.Alerted);
    }

    [Test]
    public void OutOfRangeTargetCannotBeDetectedUnlessAlreadyWanted()
    {
        var model = new NpcPerceptionModel();

        var passive = model.Evaluate(60f, 35f, 1f, false, false);
        var wanted = model.Evaluate(60f, 35f, 0.1f, true, true);

        Assert.IsFalse(passive.Detected);
        Assert.IsTrue(wanted.Detected);
        Assert.IsTrue(wanted.Alerted);
    }
}
