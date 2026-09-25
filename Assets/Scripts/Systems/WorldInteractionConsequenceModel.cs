using UnityEngine;

namespace Vanta.Systems
{
    public readonly struct WorldInteractionConsequence
    {
        public readonly bool Applied;
        public readonly float Disruption;
        public readonly float PursuitPressure;

        public WorldInteractionConsequence(bool applied, float disruption, float pursuitPressure)
        {
            Applied = applied;
            Disruption = disruption;
            PursuitPressure = pursuitPressure;
        }
    }

    public sealed class WorldInteractionConsequenceModel
    {
        public WorldInteractionConsequence Resolve(WorldInteractionDeviceType type, WorldInteractionAction action)
        {
            var disabling = action == WorldInteractionAction.Disable;
            if (!disabling)
                return new WorldInteractionConsequence(true, 0f, 0f);

            return type switch
            {
                WorldInteractionDeviceType.TrafficLight => new WorldInteractionConsequence(true, 0.8f, 0.35f),
                WorldInteractionDeviceType.SecurityGate => new WorldInteractionConsequence(true, 0.5f, 0.2f),
                WorldInteractionDeviceType.Alarm => new WorldInteractionConsequence(true, 0.7f, 0.1f),
                WorldInteractionDeviceType.Bridge => new WorldInteractionConsequence(true, 0.9f, 0.25f),
                WorldInteractionDeviceType.Camera => new WorldInteractionConsequence(true, 0.4f, 0.15f),
                _ => new WorldInteractionConsequence(false, 0f, 0f)
            };
        }
    }
}
