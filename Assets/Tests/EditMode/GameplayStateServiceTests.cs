using NUnit.Framework;
using Vanta.Core;

namespace Vanta.Tests
{
    public sealed class GameplayStateServiceTests
    {
        [Test]
        public void GameplayStateAllowsOnlyValidLifecycleTransitions()
        {
            var service = new GameplayStateService();
            Assert.That(service.TrySet(GameplayState.Paused), Is.False);
            Assert.That(service.TrySet(GameplayState.Playing), Is.True);
            Assert.That(service.TrySet(GameplayState.Paused), Is.True);
            Assert.That(service.TrySet(GameplayState.Playing), Is.True);
            Assert.That(service.TrySet(GameplayState.Dead), Is.True);
            Assert.That(service.TrySet(GameplayState.Paused), Is.False);
            Assert.That(service.TrySet(GameplayState.Playing), Is.True);
        }

        [Test]
        public void ResetRaisesChangeEventAndReturnsToMainMenu()
        {
            var service = new GameplayStateService();
            var changes = 0;
            GameplayState previous = default;
            GameplayState next = default;
            service.Changed += (from, to) =>
            {
                changes++;
                previous = from;
                next = to;
            };

            Assert.That(service.TrySet(GameplayState.Playing), Is.True);
            changes = 0;

            service.Reset();

            Assert.That(service.Current, Is.EqualTo(GameplayState.MainMenu));
            Assert.That(changes, Is.EqualTo(1));
            Assert.That(previous, Is.EqualTo(GameplayState.Playing));
            Assert.That(next, Is.EqualTo(GameplayState.MainMenu));
        }

        [Test]
        public void ResetIsIdempotentWhenAlreadyInMainMenu()
        {
            var service = new GameplayStateService();
            var changes = 0;
            service.Changed += (_, _) => changes++;

            service.Reset();

            Assert.That(changes, Is.Zero);
            Assert.That(service.Current, Is.EqualTo(GameplayState.MainMenu));
        }
    }
}
