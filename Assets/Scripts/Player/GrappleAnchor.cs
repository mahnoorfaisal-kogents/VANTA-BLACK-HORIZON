using UnityEngine;

namespace Vanta.Player
{
    public sealed class GrappleAnchor : MonoBehaviour
    {
        [SerializeField] private string anchorId;

        public string AnchorId => string.IsNullOrWhiteSpace(anchorId)
            ? gameObject.GetInstanceID().ToString()
            : anchorId;
    }
}
