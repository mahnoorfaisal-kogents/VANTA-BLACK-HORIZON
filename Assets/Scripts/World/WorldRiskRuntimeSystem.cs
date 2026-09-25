using UnityEngine;
using Vanta.Systems;

namespace Vanta.World
{
    public sealed class WorldRiskRuntimeSystem : MonoBehaviour
    {
        [SerializeField] private WorldTimeSystem worldTime;
        [SerializeField] private WantedSystem wanted;

        readonly WorldRiskModel model = new();

        public WorldRiskResult Current { get; private set; }

        void Update()
        {
            var night = worldTime && worldTime.IsNight;
            var heat = wanted ? wanted.Heat : 0f;
            Current = model.Evaluate(night, heat);
        }
    }
}
