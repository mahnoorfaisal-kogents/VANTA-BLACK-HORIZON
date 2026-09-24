using NUnit.Framework;
using Vanta.AI;
using Vanta.Missions;
using Vanta.Systems;
using Vanta.World;

namespace Vanta.Tests
{
    public sealed class AdvancedSystemsTests
    {
        [Test]
        public void MissionGraphUnlocksDependentObjectives()
        {
            var graph = new MissionObjectiveGraph();
            graph.Build(new[]
            {
                new MissionObjectiveNode { id = "infiltrate", status = ObjectiveStatus.Locked },
                new MissionObjectiveNode { id = "extract", prerequisites = new[] { "infiltrate" } }
            });
            Assert.That(graph.Nodes["infiltrate"].status, Is.EqualTo(ObjectiveStatus.Available));
            Assert.That(graph.SetActive("infiltrate"), Is.True);
            Assert.That(graph.Complete("infiltrate"), Is.True);
            Assert.That(graph.Nodes["extract"].status, Is.EqualTo(ObjectiveStatus.Available));
        }

        [Test]
        public void MissionGraphApproachTagsAreEnforced()
        {
            var graph = new MissionObjectiveGraph();
            graph.Build(new[] { new MissionObjectiveNode { id = "vault", approaches = new[] { "stealth", "combat" } } });
            Assert.That(graph.SupportsApproach("vault", "stealth"), Is.True);
            Assert.That(graph.SupportsApproach("vault", "driving"), Is.False);
        }

        [Test]
        public void TerritoryControlMovesThroughDeterministicStates()
        {
            var system = new FactionTerritorySystem();
            var zone = new TerritoryZone { zoneId = "downtown", controlValue = 0 };
            system.Register(zone);
            Assert.That(zone.Control, Is.EqualTo(TerritoryControl.Neutral));
            system.ApplyInfluence("downtown", 30, "iron_jackals");
            Assert.That(zone.Control, Is.EqualTo(TerritoryControl.Contested));
            system.ApplyInfluence("downtown", 40, "iron_jackals");
            Assert.That(zone.Control, Is.EqualTo(TerritoryControl.Allied));
        }

        [Test]
        public void StealthSuspicionEscalatesAndDecays()
        {
            var system = new StealthPerceptionSystem();
            system.AddSuspicion(30);
            Assert.That(system.State, Is.EqualTo(SuspicionState.Suspicious));
            system.AddSuspicion(50);
            Assert.That(system.State, Is.EqualTo(SuspicionState.Investigating));
            system.Tick(2f);
            Assert.That(system.suspicion, Is.EqualTo(44f).Within(0.01f));
        }

        [Test]
        public void ScoutingConsumesOneChargeOnlyWhenNewInformationIsRevealed()
        {
            var system = new ScoutingSystem();
            Assert.That(system.UseScout(new[] { "mission_a", "safehouse_a" }), Is.True);
            Assert.That(system.ScoutCharges, Is.EqualTo(2));
            Assert.That(system.UseScout(new[] { "mission_a" }), Is.False);
            Assert.That(system.ScoutCharges, Is.EqualTo(2));
        }
    }
}