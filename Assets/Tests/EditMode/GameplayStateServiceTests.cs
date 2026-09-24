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
    }
}
