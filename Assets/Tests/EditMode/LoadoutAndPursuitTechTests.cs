using NUnit.Framework;
using Vanta.Systems;

namespace Vanta.Tests
{
    public sealed class LoadoutAndPursuitTechTests
    {
        [Test]
        public void PursuitTechMustBeUnlockedBeforeUse()
        {
            var gameObject = new UnityEngine.GameObject("PursuitTechTest");
            try
            {
                var system = gameObject.AddComponent<PursuitTechSystem>();
                Assert.That(system.TryUse(PursuitTech.SpikeStrip), Is.False);
                Assert.That(system.Unlock(PursuitTech.SpikeStrip), Is.True);
                Assert.That(system.TryUse(PursuitTech.SpikeStrip), Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }
    }
}
