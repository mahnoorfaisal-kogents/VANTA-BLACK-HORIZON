using NUnit.Framework;
using Vanta.AI;
using Vanta.Combat;
using Vanta.Missions;
using Vanta.Systems;
using Vanta.Vehicles;
using Vanta.World;

namespace Vanta.Tests
{
    public sealed class SystemArchitectureHardeningTests
    {
        [Test]
        public void EnemyStateMachineRejectsIllegalTransitions()
        {
            var machine = new EnemyStateMachine();
            Assert.That(machine.TryTransition(EnemyState.Combat), Is.False);
            Assert.That(machine.Current, Is.EqualTo(EnemyState.Idle));
            Assert.That(machine.TryTransition(EnemyState.Alert), Is.True);
            Assert.That(machine.TryTransition(EnemyState.Combat), Is.True);
            Assert.That(machine.TryTransition(EnemyState.Dead), Is.True);
            Assert.That(machine.TryTransition(EnemyState.Combat), Is.False);
        }

        [Test]
        public void MissionCannotCompleteUntilRequiredObjectivesAreComplete()
        {
            var graph = new MissionObjectiveGraph();
            graph.Build(new[]
            {
                new MissionObjectiveNode { id = "required", Optional = false },
                new MissionObjectiveNode { id = "optional", Optional = true }
            });
            Assert.That(graph.CanCompleteMission(), Is.False);
            Assert.That(graph.SetActive("required"), Is.True);
            Assert.That(graph.Complete("required"), Is.True);
            Assert.That(graph.CanCompleteMission(), Is.True);
        }

        [Test]
        public void PersistentWorldSnapshotUsesSerializableInfluenceEntries()
        {
            var state = new PersistentWorldState();
            var snapshot = state.Capture(8.5f, WeatherSystem.WeatherState.Clear,
                new[] { "a" }, new[] { "m1" }, new System.Collections.Generic.Dictionary<string, int> { ["north"] = 42 });
            Assert.That(snapshot.territoryInfluenceEntries.Count, Is.EqualTo(1));
            Assert.That(snapshot.territoryInfluenceEntries[0].districtId, Is.EqualTo("north"));
            Assert.That(snapshot.territoryInfluenceEntries[0].influence, Is.EqualTo(42));
        }

        [Test]
        public void VehicleDamageClampsAndRaisesDestroyedOnce()
        {
            var state = new VehicleDamageState(100f);
            var destroyed = 0;
            state.Destroyed += () => destroyed++;
            Assert.That(state.ApplyDamage(30f), Is.EqualTo(70f));
            Assert.That(state.ApplyDamage(100f), Is.EqualTo(0f));
            Assert.That(state.ApplyDamage(10f), Is.EqualTo(0f));
            Assert.That(destroyed, Is.EqualTo(1));
        }

        [Test]
        public void PerformanceBudgetRejectsPopulationAtCapacity()
        {
            var budget = new PerformanceBudgetSystem();
            Assert.That(budget.CanSpawnTraffic(31), Is.True);
            Assert.That(budget.CanSpawnTraffic(32), Is.False);
            Assert.That(budget.CanSpawnCivilian(47), Is.True);
            Assert.That(budget.CanSpawnCivilian(48), Is.False);
            Assert.That(budget.IsWithinBudget(32, 48), Is.True);
            Assert.That(budget.IsWithinBudget(33, 48), Is.False);
        }

        [Test]
        public void PerformanceBudgetClampsInvalidConfiguration()
        {
            var budget = new PerformanceBudgetSystem();
            var target = typeof(PerformanceBudgetSystem).GetField("targetFrameRate",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            target.SetValue(budget, 0);
            var validate = typeof(PerformanceBudgetSystem).GetMethod("OnValidate",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            validate.Invoke(budget, null);
            Assert.That(budget.TargetFrameRate, Is.EqualTo(1));
        }

        [Test]
        public void AgentMemoryRecallsRelevantFactsAndEvictsLeastImportant()
        {
            var memory = new Vanta.AI.AgentMemoryStore(2);
            Assert.That(memory.Remember("district", "Black Horizon market is under faction pressure", 0.9f, 1), Is.True);
            Assert.That(memory.Remember("mission", "Recover the encrypted ledger", 0.8f, 2), Is.True);
            Assert.That(memory.Remember("low", "temporary rumor", 0.1f, 3), Is.True);
            Assert.That(memory.Count, Is.EqualTo(2));
            Assert.That(memory.RecallRelevant("encrypted ledger").Count, Is.EqualTo(1));
            Assert.That(memory.RecallRelevant("encrypted ledger")[0].key, Is.EqualTo("mission"));
        }

        [Test]
        public void AgentDecisionSystemPrioritizesThreatAndDeterministicallyBreaksTies()
        {
            var flee = Vanta.AI.AgentDecisionSystem.Choose(new Vanta.AI.AgentDecisionContext(1f, 0f, 0f, false, false));
            Assert.That(flee, Is.EqualTo(Vanta.AI.AgentAction.Flee));

            var patrol = Vanta.AI.AgentDecisionSystem.Choose(new Vanta.AI.AgentDecisionContext(0f, 0f, 0f, false, false));
            Assert.That(patrol, Is.EqualTo(Vanta.AI.AgentAction.Patrol));
        }

        [Test]
        public void PopulationModelsRespectSharedPerformanceBudgets()
        {
            var budget = new PerformanceBudgetPolicy(2, 3);
            var traffic = new TrafficPopulationModel(budget.MaxActiveTraffic);
            var civilians = new CivilianPopulationModel(budget.MaxActiveCivilians);

            Assert.That(traffic.TrySpawn("car-1"), Is.True);
            Assert.That(traffic.TrySpawn("car-2"), Is.True);
            Assert.That(traffic.TrySpawn("car-3"), Is.False);
            Assert.That(civilians.TrySpawn("ped-1"), Is.True);
            Assert.That(civilians.TrySpawn("ped-2"), Is.True);
            Assert.That(civilians.TrySpawn("ped-3"), Is.True);
            Assert.That(civilians.TrySpawn("ped-4"), Is.False);
            Assert.That(budget.IsWithinBudget(traffic.ActiveCount, civilians.ActiveCount), Is.True);
        }

        [Test]
        public void DistrictStreamingRejectsIllegalLifecycleTransitions()
        {
            var streaming = new DistrictStreamingSystem();
            Assert.That(streaming.CompleteLoad("north"), Is.False);
            Assert.That(streaming.BeginLoad("north"), Is.True);
            Assert.That(streaming.BeginLoad("north"), Is.False);
            Assert.That(streaming.CompleteLoad("north"), Is.True);
            Assert.That(streaming.BeginUnload("north"), Is.True);
            Assert.That(streaming.CompleteUnload("north"), Is.True);
            Assert.That(streaming.GetState("north"), Is.EqualTo(DistrictStreamState.Unloaded));
        }

        [Test]
        public void WeaponAmmoStateSupportsDeterministicFireAndReload()
        {
            var ammo = new WeaponAmmoState(3);
            Assert.That(ammo.TryConsumeRound(), Is.True);
            Assert.That(ammo.TryConsumeRound(), Is.True);
            Assert.That(ammo.Remaining, Is.EqualTo(1));
            Assert.That(ammo.TryConsumeRound(), Is.True);
            Assert.That(ammo.TryConsumeRound(), Is.False);
            ammo.Reload();
            Assert.That(ammo.Remaining, Is.EqualTo(3));
        }
    }
}
