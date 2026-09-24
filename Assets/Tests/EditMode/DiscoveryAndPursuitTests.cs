using NUnit.Framework;
using Vanta.AI;

namespace Vanta.Tests
{
    public sealed class DiscoveryAndPursuitTests
    {
        [Test]
        public void PursuitStartsDormantAndTransitionsDeterministically()
        {
            var machine = new PursuitStateMachine();
            Assert.That(machine.Current, Is.EqualTo(PursuitState.Dormant));
            machine.Dispatch();
            Assert.That(machine.Current, Is.EqualTo(PursuitState.Dispatching));
            machine.BeginIntercept();
            Assert.That(machine.Current, Is.EqualTo(PursuitState.Intercepting));
            machine.LoseTarget();
            Assert.That(machine.Current, Is.EqualTo(PursuitState.Searching));
            machine.BeginCooldown();
            Assert.That(machine.Current, Is.EqualTo(PursuitState.Cooldown));
            machine.Reset();
            Assert.That(machine.Current, Is.EqualTo(PursuitState.Dormant));
        }
    }
}
