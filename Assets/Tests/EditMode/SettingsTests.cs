using NUnit.Framework;
using Vanta.UI;

namespace Vanta.Tests
{
    public sealed class SettingsTests
    {
        [Test]
        public void SettingsClampRuntimeValues()
        {
            var system = new VantaSettingsSystem();
            system.Apply(new VantaSettingsData
            {
                lookSensitivity = 9f,
                masterVolume = -1f,
                musicVolume = 2f,
                effectsVolume = 0.5f
            });

            Assert.That(system.Data.lookSensitivity, Is.EqualTo(3f));
            Assert.That(system.Data.masterVolume, Is.EqualTo(0f));
            Assert.That(system.Data.musicVolume, Is.EqualTo(1f));
            Assert.That(system.Data.effectsVolume, Is.EqualTo(0.5f));
        }
    }
}