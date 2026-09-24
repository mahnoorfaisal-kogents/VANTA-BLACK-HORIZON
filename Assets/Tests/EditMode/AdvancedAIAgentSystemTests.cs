using NUnit.Framework;
using Vanta.AI;

public sealed class AdvancedAIAgentSystemTests
{
    [Test]
    public void GoalSystemChoosesHighestUtilityGoalDeterministically()
    {
        var goals = new AgentGoalSystem();
        goals.AddOrUpdate(new AgentGoal("survive", 0.4f));
        goals.AddOrUpdate(new AgentGoal("protect", 0.9f));
        goals.AddOrUpdate(new AgentGoal("patrol", 0.2f));

        Assert.AreEqual("protect", goals.SelectGoal().Id);
    }

    [Test]
    public void GoalSystemIgnoresBlockedGoals()
    {
        var goals = new AgentGoalSystem();
        goals.AddOrUpdate(new AgentGoal("blocked", 1f, false));
        goals.AddOrUpdate(new AgentGoal("patrol", 0.5f, true));

        Assert.AreEqual("patrol", goals.SelectGoal().Id);
    }

    [Test]
    public void TickBudgetUsesStablePriorityAndNeverExceedsCapacity()
    {
        var budget = new AITickBudget(2);
        Assert.IsTrue(budget.TryAcquire(0, 0.9f));
        Assert.IsTrue(budget.TryAcquire(1, 0.8f));
        Assert.IsFalse(budget.TryAcquire(2, 1f));
        Assert.AreEqual(2, budget.Used);
    }

    [Test]
    public void KnowledgeBaseReturnsOnlyKnownFacts()
    {
        var kb = new AgentKnowledgeBase();
        kb.AddFact("safehouse", "old station", 0.9f);
        kb.AddFact("territory", "north docks", 0.7f);

        Assert.IsTrue(kb.TryGetFact("safehouse", out var value));
        Assert.AreEqual("old station", value);
        Assert.IsFalse(kb.TryGetFact("unknown", out _));
    }

    [Test]
    public void ActionExecutorRejectsDeadAgentsAndRecordsValidActions()
    {
        var executor = new AgentActionExecutor();
        Assert.IsFalse(executor.TryExecute(AgentAction.Pursue, false));
        Assert.IsTrue(executor.TryExecute(AgentAction.Investigate, true));
        Assert.AreEqual(AgentAction.Investigate, executor.LastAction);
    }

    [Test]
    public void DirectorEmitsDeterministicPressureWithoutNegativeValues()
    {
        var director = new WorldSimulationDirector(1234);
        var pressure = director.CalculatePressure(0.8f, 0.2f, 0.6f);
        Assert.GreaterOrEqual(pressure, 0f);
        Assert.LessOrEqual(pressure, 1f);
        Assert.AreEqual(pressure, director.CalculatePressure(0.8f, 0.2f, 0.6f), 0.0001f);
    }
}
