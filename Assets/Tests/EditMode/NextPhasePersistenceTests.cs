using NUnit.Framework;
using Vanta.Missions;
using Vanta.Save;

namespace Vanta.Tests
{
    public sealed class NextPhasePersistenceTests
    {
        [Test]
        public void MissionRuntimeSnapshotRestoresObjectiveProgress()
        {
            var system = new MissionSystem();
            var definition = UnityEngine.ScriptableObject.CreateInstance<MissionDefinition>();
            definition.missionId = "persistence-test";
            definition.objectiveGraph = new[]
            {
                new MissionObjectiveNode { id = "approach" },
                new MissionObjectiveNode { id = "finish", prerequisites = new[] { "approach" } }
            };

            Assert.That(system.StartMission(definition), Is.True);
            Assert.That(system.SetObjectiveActive(definition.missionId, "approach"), Is.True);
            Assert.That(system.CompleteObjective(definition.missionId, "approach"), Is.True);

            var snapshot = system.CaptureSaveState();
            var restored = new MissionSystem();
            Assert.That(restored.RestoreSaveState(snapshot, new[] { definition }), Is.True);
            Assert.That(restored.ActiveMissionId, Is.EqualTo(definition.missionId));
            Assert.That(restored.GetObjectiveStatus(definition.missionId, "approach"), Is.EqualTo(ObjectiveStatus.Complete));
            Assert.That(restored.GetObjectiveStatus(definition.missionId, "finish"), Is.EqualTo(ObjectiveStatus.Available));

            UnityEngine.Object.DestroyImmediate(definition);
        }

        [Test]
        public void SaveDataCanRoundTripActiveMissionSnapshot()
        {
            var data = new SaveData { activeMission = "mission-x", missions = new System.Collections.Generic.List<MissionSaveState>
            {
                new MissionSaveState { id = "mission-x", status = (int)MissionSystem.Status.Active, objectiveIndex = 2 }
            }};
            var json = UnityEngine.JsonUtility.ToJson(data);
            var restored = UnityEngine.JsonUtility.FromJson<SaveData>(json);
            Assert.That(restored.activeMission, Is.EqualTo("mission-x"));
            Assert.That(restored.missions.Count, Is.EqualTo(1));
            Assert.That(restored.missions[0].objectiveIndex, Is.EqualTo(2));
        }
    }
}
