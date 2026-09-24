using NUnit.Framework;
using Vanta.AI;
using Vanta.Combat;
using Vanta.Missions;
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
