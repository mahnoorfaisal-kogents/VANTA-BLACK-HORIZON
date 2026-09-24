using NUnit.Framework;
using Vanta.Save;
using Vanta.Systems;
using Vanta.World;

namespace Vanta.Tests
{
    public sealed class NextWaveSystemsTests
    {
        [Test]
        public void ActivityRequiresLevelAndCanTransition()
        {
            var system = new ActivitySystem();
            var locked = system.Register(new ActivityDefinition { activityId = "raid", requiredLevel = 3 }, 2, 0);
            Assert.That(locked.State, Is.EqualTo(ActivityState.Locked));

            var available = system.Register(new ActivityDefinition { activityId = "raid_2", requiredLevel = 2 }, 2, 0);
            Assert.That(available.State, Is.EqualTo(ActivityState.Available));
            Assert.That(system.Start("raid_2"), Is.True);
            Assert.That(system.Complete("raid_2"), Is.True);
            Assert.That(system.GetState("raid_2"), Is.EqualTo(ActivityState.Complete));
        }

        [Test]
        public void DistrictUnlockAndActivationAreDeterministic()
        {
            var system = new DistrictRuntimeSystem();
            var definition = UnityEngine.ScriptableObject.CreateInstance<DistrictDefinition>();
            definition.districtId = "sector_a";
            system.Register(definition, true);
            Assert.That(system.IsUnlocked("sector_a"), Is.True);
            Assert.That(system.SetActive("sector_a"), Is.True);
            Assert.That(system.ActiveDistrictId, Is.EqualTo("sector_a"));
            UnityEngine.Object.DestroyImmediate(definition);
        }

        [Test]
        public void DialogueRejectsUnavailableChoice()
        {
            var runtime = new DialogueRuntime();
            runtime.Build(new[]
            {
                new DialogueNode
                {
                    id = "start",
                    choices = new[] { new DialogueChoice { id = "locked", nextNodeId = "end", requiredReputation = 20 } }
                },
                new DialogueNode { id = "end" }
            });
            Assert.That(runtime.Start("start"), Is.True);
            Assert.That(runtime.Choose("locked", 19), Is.False);
            Assert.That(runtime.CurrentNodeId, Is.EqualTo("start"));
            Assert.That(runtime.Choose("locked", 20), Is.True);
            Assert.That(runtime.CurrentNodeId, Is.EqualTo("end"));
        }

        [Test]
        public void InteractionRequiresRegisteredOption()
        {
            var system = new InteractionSystem();
            Assert.That(system.TryInteract("garage"), Is.False);
            Assert.That(system.Register("garage", "Use Garage"), Is.True);
            Assert.That(system.TryInteract("garage"), Is.True);
            Assert.That(system.Count, Is.EqualTo(1));
        }

        [Test]
        public void SaveSlotsSanitizePathSeparators()
        {
            var save = new SaveSystem();
            var path = save.SlotPath("../slot");
            Assert.That(path, Does.Not.Contain("../"));
            Assert.That(path, Does.Not.Contain("..\\"));
        }
    }
}