using NUnit.Framework;
using Vanta.Systems;
using Vanta.World;

namespace Vanta.Tests
{
    public sealed class PerformanceAndStreamingTests
    {
        [Test]
        public void PoolReusesReturnedObjectsWithinCapacity()
        {
            var created = 0;
            var pool = new RuntimePool<object>(() => { created++; return new object(); }, 1);
            var first = pool.Rent();
            pool.Return(first);
            var second = pool.Rent();
            Assert.That(second, Is.SameAs(first));
            Assert.That(created, Is.EqualTo(1));
        }

        [Test]
        public void DistrictStreamingFollowsValidLifecycle()
        {
            var system = new DistrictStreamingSystem();
            Assert.That(system.BeginLoad("district_a"), Is.True);
            Assert.That(system.CompleteLoad("district_a"), Is.True);
            Assert.That(system.BeginUnload("district_a"), Is.True);
            Assert.That(system.CompleteUnload("district_a"), Is.True);
            Assert.That(system.GetState("district_a"), Is.EqualTo(DistrictStreamState.Unloaded));
        }

        [Test]
        public void DistrictStreamingRejectsInvalidTransitions()
        {
            var system = new DistrictStreamingSystem();
            Assert.That(system.CompleteLoad("district_a"), Is.False);
            Assert.That(system.BeginUnload("district_a"), Is.False);
        }

        [Test]
        public void PerformanceBudgetIsDeterministic()
        {
            var system = new PerformanceBudgetSystem();
            Assert.That(system.IsWithinBudget(16, 20), Is.True);
            Assert.That(system.IsWithinBudget(33, 20), Is.False);
            Assert.That(system.IsWithinBudget(16, 49), Is.False);
        }
    }
}