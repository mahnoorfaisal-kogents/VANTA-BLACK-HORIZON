using System;
using Vanta.Systems;

namespace Vanta.Player
{
    public enum TraversalAction
    {
        None,
        Vault,
        Grapple
    }

    public sealed class VerticalTraversalModel
    {
        readonly GrappleTraversalModel grapple;

        public VerticalTraversalModel(float grappleRange)
        {
            grapple = new GrappleTraversalModel(grappleRange);
        }

        public TraversalAction Resolve(bool grappleAnchorAvailable, float anchorDistance, bool vaultAvailable)
        {
            if (grappleAnchorAvailable && anchorDistance >= 0f)
            {
                var anchorId = "runtime-anchor";
                if (grapple.TryAttach(anchorDistance, anchorId))
                {
                    grapple.Detach();
                    return TraversalAction.Grapple;
                }
            }

            return vaultAvailable ? TraversalAction.Vault : TraversalAction.None;
        }
    }
}
