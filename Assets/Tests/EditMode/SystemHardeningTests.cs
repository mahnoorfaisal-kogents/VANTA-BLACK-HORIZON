using NUnit.Framework;
using Vanta.AI;
using Vanta.World;

namespace Vanta.Tests
{
    public sealed class SystemHardeningTests
    {
        [Test]
        public void PursuitEscalationMatchesWantedThresholds()
        {
            Assert.That(PolicePursuitCoordinator.ResolveEscalation(0), Is.EqualTo(PoliceEscalationLevel.Patrol));
            Assert.That(PolicePursuitCoordinator.ResolveEscalation(2), Is.EqualTo(PoliceEscalationLevel.Response));
            Assert.That(PolicePursuitCoordinator.ResolveEscalation(4), Is.EqualTo(PoliceEscalationLevel.Tactical));
            Assert.That(PolicePursuitCoordinator.ResolveEscalation(5), Is.EqualTo(PoliceEscalationLevel.Major));
        }

        [Test]
        public void NPCScheduleSupportsOvernightRanges()
        {
            Assert.That(NPCScheduleSystem.IsWithin(23.5f, 22, 6), Is.True);
            Assert.That(NPCScheduleSystem.IsWithin(5.9f, 22, 6), Is.True);
            Assert.That(NPCScheduleSystem.IsWithin(12f, 22, 6), Is.False);
        }

        [Test]
        public void TrafficCapacityRejectsDuplicateAndOverflowSpawns()
        {
            var model = new TrafficPopulationModel(1);
            Assert.That(model.TrySpawn("car-1"), Is.True);
            Assert.That(model.TrySpawn("car-1"), Is.False);
            Assert.That(model.TrySpawn("car-2"), Is.False);
            Assert.That(model.Despawn("car-1"), Is.True);
            Assert.That(model.TrySpawn("car-2"), Is.True);
        }

        [Test]
        public void WorldEventLifecycleIsOneWay()
        {
            var runtime = new WorldEventRuntime();
            Assert.That(runtime.TryResolve(), Is.False);
            Assert.That(runtime.TryActivate(), Is.True);
            Assert.That(runtime.TryActivate(), Is.False);
            Assert.That(runtime.TryResolve(), Is.True);
            Assert.That(runtime.TryFail(), Is.False);
        }
    }
}