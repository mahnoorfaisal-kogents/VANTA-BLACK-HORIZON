using UnityEngine;

namespace Vanta.Racing
{
    [RequireComponent(typeof(Collider))]
    public sealed class RacingCheckpoint : MonoBehaviour
    {
        [SerializeField] private RacingActivitySystem race;
        [SerializeField] private string checkpointId;

        void Reset()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!race || !other.CompareTag("Player"))
                return;

            race.PassCheckpoint(checkpointId);
        }
    }
}
