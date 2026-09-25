using System;
using UnityEngine;

namespace Vanta.Systems
{
    public sealed class WorldInteractionDevice : MonoBehaviour
    {
        [SerializeField] private string deviceId;
        [SerializeField] private WorldInteractionDeviceType deviceType;
        [SerializeField] private bool enabledState = true;

        private WorldInteractionModel model;
        private readonly WorldInteractionConsequenceModel consequenceModel = new();
        private readonly WorldInteractionCrimeModel crimeModel = new();
        [SerializeField] private WantedSystem wanted;

        public string DeviceId => deviceId;
        public WorldInteractionDeviceType DeviceType => deviceType;
        public bool IsEnabled => model != null && model.IsEnabled(deviceId);
        public float LastDisruption { get; private set; }
        public float LastPursuitPressure { get; private set; }
        public event Action<WorldInteractionConsequence> ConsequenceApplied;

        public void Configure(string id, WorldInteractionDeviceType type, bool enabled = true)
        {
            deviceId = id;
            deviceType = type;
            enabledState = enabled;
        }

        public void Initialize(WorldInteractionModel sharedModel)
        {
            model = sharedModel;
            if (model == null || string.IsNullOrWhiteSpace(deviceId))
                return;

            model.Register(deviceId, deviceType, enabledState);
        }

        public bool Interact()
        {
            return SetEnabled(!IsEnabled);
        }

        public bool SetEnabled(bool value)
        {
            if (model == null || string.IsNullOrWhiteSpace(deviceId))
                return false;

            var action = value ? WorldInteractionAction.Enable : WorldInteractionAction.Disable;
            if (!model.TryApply(deviceId, action))
                return false;

            var consequence = consequenceModel.Resolve(deviceType, action);
            LastDisruption = consequence.Disruption;
            LastPursuitPressure = consequence.PursuitPressure;
            ConsequenceApplied?.Invoke(consequence);
            if (consequence.PursuitPressure > 0f && wanted)
                wanted.AddCrime(crimeModel.ResolveCrime(consequence.PursuitPressure));
            return true;
        }
    }
}
