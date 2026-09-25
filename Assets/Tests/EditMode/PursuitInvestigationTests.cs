using NUnit.Framework;
using Vanta.AI;

public sealed class PursuitInvestigationTests
{
    [Test]
    public void PursuitCanEnterInvestigationFromDispatch()
    {
        var machine = new PursuitStateMachine();

        Assert.IsTrue(machine.Dispatch());
        Assert.IsTrue(machine.BeginInvestigation());
        Assert.AreEqual(PursuitState.Investigating, machine.Current);
    }

    [Test]
    public void InvestigationCanEscalateOrCoolDown()
    {
        var machine = new PursuitStateMachine();

        machine.Dispatch();
        machine.BeginInvestigation();

        Assert.IsTrue(machine.BeginIntercept());
        Assert.AreEqual(PursuitState.Intercepting, machine.Current);

        machine.LoseTarget();
        machine.BeginCooldown();
        Assert.AreEqual(PursuitState.Cooldown, machine.Current);
    }
}
