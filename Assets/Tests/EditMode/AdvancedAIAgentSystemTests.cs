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
    public void SquadSelectsHighestPriorityLeaderOrAssaultMember()
    {
        var squad = new AgentSquadSystem();
        squad.AddOrUpdate(new SquadMember(7, SquadRole.Assault, 0.4f));
        squad.AddOrUpdate(new SquadMember(3, SquadRole.Leader, 0.9f));
        squad.AddOrUpdate(new SquadMember(2, SquadRole.Support, 1f));
        Assert.AreEqual(3, squad.SelectLeader().Id);
    }

    [Test]
    public void TelemetryRemainsBounded()
    {
        var telemetry = new AgentTelemetry(2);
        telemetry.Record(new AgentDecisionTrace(1, AgentAction.Patrol, 0.1f));
        telemetry.Record(new AgentDecisionTrace(2, AgentAction.Investigate, 0.2f));
        telemetry.Record(new AgentDecisionTrace(3, AgentAction.Pursue, 0.8f));
        Assert.AreEqual(2, telemetry.Count);
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
    [Test]
    public void TacticalSystemMapsThreatToDeterministicGoal()
    {
        var context = new AgentTacticalContext(0.9f, 0.8f, 0.1f, true, true, true);
        var result = AgentTacticalSystem.Evaluate(context);

        Assert.AreEqual(AgentTacticalGoal.Survive, result.Goal);
        Assert.AreEqual(AgentAction.Flee, result.Action);
    }

    [Test]
    public void TacticalSystemUsesInvestigationWhenTargetIsNotVisible()
    {
        var context = new AgentTacticalContext(0.2f, 0.9f, 0.2f, true, false, true);
        var result = AgentTacticalSystem.Evaluate(context);

        Assert.AreEqual(AgentTacticalGoal.Investigate, result.Goal);
        Assert.AreEqual(AgentAction.Investigate, result.Action);
    }

    [Test]
    public void TacticalSystemProducesBoundedDeterministicScores()
    {
        var context = new AgentTacticalContext(0.55f, 0.35f, 0.65f, true, true, true);
        var first = AgentTacticalSystem.Evaluate(context);
        var second = AgentTacticalSystem.Evaluate(context);

        Assert.AreEqual(first.Goal, second.Goal);
        Assert.AreEqual(first.Action, second.Action);
        Assert.GreaterOrEqual(first.GoalUtility, 0f);
        Assert.LessOrEqual(first.GoalUtility, 1f);
    }

    [Test]
    public void AIDirectorCalculatesTierAndEmitsContextualEventDeterministically()
    {
        var director = new AIDirectorModel(77);
        var result = director.Evaluate(0.9f, 0.8f, 0.6f);

        Assert.GreaterOrEqual(result.Pressure, 0f);
        Assert.LessOrEqual(result.Pressure, 1f);
        Assert.AreEqual(3, result.EventTier);
        Assert.AreEqual(result.EventTier, director.Evaluate(0.9f, 0.8f, 0.6f).EventTier);
    }

    [Test]
    public void SquadCoordinatorAssignsDistinctDeterministicRoles()
    {
        var coordinator = new AgentSquadCoordinator(4);
        coordinator.Add(1, 0.9f);
        coordinator.Add(2, 0.7f);
        coordinator.Add(3, 0.5f);

        var assignments = coordinator.BuildAssignments();

        Assert.AreEqual(SquadRole.Leader, assignments[0].Role);
        Assert.AreEqual(SquadRole.Assault, assignments[1].Role);
        Assert.AreEqual(SquadRole.Support, assignments[2].Role);
    }

}
