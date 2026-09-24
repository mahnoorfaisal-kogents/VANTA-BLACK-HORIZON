using NUnit.Framework;
using Vanta.AI;

namespace Vanta.Tests
{
    public sealed class EnemyStateMachineTests
    {
        [Test] public void StartsIdle() => Assert.That(new EnemyStateMachine().Current, Is.EqualTo(EnemyState.Idle));

        [Test]
        public void TransitionChangesCurrentState()
        {
            var machine = new EnemyStateMachine();
            Assert.That(machine.Transition(EnemyState.Combat), Is.True);
            Assert.That(machine.Current, Is.EqualTo(EnemyState.Combat));
        }

        [Test]
        public void DeadEnemyCannotReturnToCombat()
        {
            var machine = new EnemyStateMachine();
            Assert.That(machine.Transition(EnemyState.Dead), Is.True);
            Assert.That(machine.Transition(EnemyState.Combat), Is.False);
            Assert.That(machine.Current, Is.EqualTo(EnemyState.Dead));
        }

        [Test]
        public void IdleCannotJumpDirectlyToChase()
        {
            var machine = new EnemyStateMachine();
            Assert.That(machine.Transition(EnemyState.Chase), Is.False);
            Assert.That(machine.Current, Is.EqualTo(EnemyState.Idle));
        }
    }
}