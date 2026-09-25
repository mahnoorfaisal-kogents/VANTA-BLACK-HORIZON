using NUnit.Framework;
using UnityEngine;
using Vanta.Missions;

public sealed class MissionObjectiveTriggerSetTests
{
    [Test]
    public void CompletingFinalObjectiveCompletesMission()
    {
        var root = new GameObject("MissionRuntimeTest");
        var system = root.AddComponent<MissionSystem>();
        var definition = ScriptableObject.CreateInstance<MissionDefinition>();
        definition.missionId = "test_mission";
        definition.objectiveGraph = new[]
        {
            new MissionObjectiveNode { id = "one", title = "One" },
            new MissionObjectiveNode { id = "two", title = "Two", prerequisites = new[] { "one" } }
        };

        Assert.IsTrue(system.StartMission(definition));
        Assert.IsTrue(system.SetObjectiveActive("test_mission", "one"));
        Assert.IsTrue(system.CompleteObjective("test_mission", "one"));
        Assert.IsTrue(system.SetObjectiveActive("test_mission", "two"));
        Assert.IsTrue(system.CompleteObjective("test_mission", "two"));

        var triggerSet = root.AddComponent<MissionObjectiveTriggerSet>();
        triggerSet.Configure(system, "test_mission", new[] { "one", "two" });
        triggerSet.TryActivateNext("two");

        Assert.AreEqual(MissionSystem.Status.Complete, system.GetStatus("test_mission"));

        Object.DestroyImmediate(definition);
        Object.DestroyImmediate(root);
    }
}
