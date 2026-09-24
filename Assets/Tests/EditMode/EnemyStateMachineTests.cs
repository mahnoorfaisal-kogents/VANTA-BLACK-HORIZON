using NUnit.Framework;
using Vanta.AI;

namespace Vanta.Tests
{
    public sealed class EnemyStateMachineTests
    {
        [Test] public void StartsIdle() => Assert.That(new EnemyStateMachine().Current, Is.EqualTo(EnemyState.Idle));
        [Test] public void TransitionChangesCurrentState()
        {
            var machine = new EnemyStateMachine();
            machine.Transition(EnemyState.Combat);
            Assert.That(machine.Current, Is.EqualTo(EnemyState.Combat));
        }
    }
}
