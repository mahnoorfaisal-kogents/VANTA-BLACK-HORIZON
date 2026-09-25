using NUnit.Framework;
using UnityEngine;
using Vanta.Player;
using Vanta.Vehicles;

public sealed class VehicleInteractionRuntimeTests
{
    [Test]
    public void EnterDisablesOnFootControllerAndExitRestoresIt()
    {
        var vehicle = new GameObject("Vehicle");
        vehicle.AddComponent<Rigidbody>();
        vehicle.AddComponent<VehicleController>();
        var interaction = vehicle.AddComponent<VehicleInteraction>();

        var player = new GameObject("Player");
        player.AddComponent<CharacterController>();
        var controller = player.AddComponent<PlayerController>();
        var characterController = player.GetComponent<CharacterController>();

        Assert.IsTrue(controller.enabled);
        Assert.IsTrue(characterController.enabled);
        Assert.IsTrue(interaction.TryEnter(player.transform));
        Assert.IsTrue(interaction.IsOccupied);
        Assert.IsFalse(controller.enabled);
        Assert.IsFalse(characterController.enabled);

        interaction.Exit(player.transform);
        Assert.IsFalse(interaction.IsOccupied);
        Assert.IsTrue(controller.enabled);
        Assert.IsTrue(characterController.enabled);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(vehicle);
    }
}
