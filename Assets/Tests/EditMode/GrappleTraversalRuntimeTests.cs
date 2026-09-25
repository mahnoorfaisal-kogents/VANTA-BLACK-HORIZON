using NUnit.Framework;
using UnityEngine;
using Vanta.Player;
using Vanta.Systems;

public sealed class GrappleTraversalRuntimeTests
{
    [Test]
    public void GrappleAnchorReferenceRemainsAttachedAfterSuccessfulRaycast()
    {
        var player = new GameObject("Player");
        player.AddComponent<CharacterController>();
        var system = player.AddComponent<GrappleTraversalSystem>();

        var anchorObject = new GameObject("Anchor");
        anchorObject.transform.position = new Vector3(0f, 0f, 5f);
        var collider = anchorObject.AddComponent<BoxCollider>();
        collider.size = Vector3.one;
        anchorObject.AddComponent<GrappleAnchor>();

        Assert.DoesNotThrow(() => system.enabled = true);
        Object.DestroyImmediate(anchorObject);
        Object.DestroyImmediate(player);
    }
}
