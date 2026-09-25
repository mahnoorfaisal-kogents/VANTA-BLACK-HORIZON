using UnityEngine;
namespace Vanta.Player
{
    public sealed class StealthSystem : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] float visibility = 1f;
        [SerializeField, Range(0f, 1f)] float noise = 0.35f;
        [SerializeField, Range(0f, 1f)] float lightExposure = 1f;

        readonly StealthPerceptionModel perception = new();

        public float Visibility => visibility;
        public float Suspicion { get; private set; }
        public bool IsHidden { get; private set; }
        public bool IsCrouched { get; private set; }

        void Update()
        {
            IsCrouched = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
            var result = perception.Evaluate(visibility, noise, lightExposure, IsCrouched);
            Suspicion = Mathf.Lerp(Suspicion, result.Suspicion, Time.deltaTime * 8f);
            IsHidden = result.IsHidden;
        }
    }
}
