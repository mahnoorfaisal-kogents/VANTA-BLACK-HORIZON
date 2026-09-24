using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Vanta.AI;
using Vanta.Systems;
using Vanta.World;
using Vanta.Missions;

namespace Vanta.Tests
{
    public sealed class SystemIntegrationTests
    {
        [Test] public void MissionConsequencesApplyRewardsAndReputation()
        {
            var c = new MissionConsequenceSystem();
            var result = c.Resolve(new MissionConsequence { cash = 250, factionId = "Iron Jackals", reputationDelta = 15, territoryId = "dock", territoryDelta = 10 });
            Assert.That(result.Cash, Is.EqualTo(250));
            Assert.That(result.ReputationDelta, Is.EqualTo(15));
            Assert.That(result.TerritoryDelta, Is.EqualTo(10));
        }

        [Test] public void WorldEventTransitionsDeterministically()
        {
            var e = new WorldEventRuntime();
            Assert.That(e.TryActivate(), Is.True);
            Assert.That(e.State, Is.EqualTo(WorldEventState.Active));
            Assert.That(e.TryResolve(), Is.True);
            Assert.That(e.State, Is.EqualTo(WorldEventState.Resolved));
        }

        [Test] public void FactionWorldEffectsResolveDangerAndMissionAccess()
        {
            var e = FactionWorldEffects.Evaluate(TerritoryControl.Hostile, 80);
            Assert.That(e.DangerMultiplier, Is.GreaterThan(1f));
            Assert.That(e.MissionAccess, Is.EqualTo(WorldMissionAccess.Restricted));
        }

        [Test] public void TrafficPopulationCapsAndReusesSlots()
        {
            var p = new TrafficPopulationModel(2);
            Assert.That(p.TrySpawn("car-a"), Is.True);
            Assert.That(p.TrySpawn("car-b"), Is.True);
            Assert.That(p.TrySpawn("car-c"), Is.False);
            Assert.That(p.Despawn("car-a"), Is.True);
            Assert.That(p.TrySpawn("car-c"), Is.True);
        }

        [Test] public void IntelMapRevealsMarkersOnce()
        {
            var map = new IntelMapSystem();
            Assert.That(map.Reveal("mission-1", WorldMarker.MarkerType.Mission), Is.True);
            Assert.That(map.Reveal("mission-1", WorldMarker.MarkerType.Mission), Is.False);
            Assert.That(map.IsRevealed("mission-1"), Is.True);
        }

        [Test] public void ProgressionUnlocksByXpThreshold()
        {
            var p = new ProgressionSystem();
            p.AddXp(1000);
            Assert.That(p.Level, Is.GreaterThanOrEqualTo(2));
        }

        [Test] public void PoliceCoordinatorMapsWantedToEscalation()
        {
            var c = new PolicePursuitCoordinator();
            c.Update(5);
            Assert.That(c.Escalation, Is.EqualTo(PoliceEscalationLevel.Major));
            Assert.That(c.PursuitState, Is.EqualTo(PursuitState.Intercepting));
        }
    }
}