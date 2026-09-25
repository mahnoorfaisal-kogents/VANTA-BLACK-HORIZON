using UnityEngine;

namespace Vanta.Systems
{
    public sealed class WorldInteractionDevice : MonoBehaviour
    {
        [SerializeField] private string deviceId;
        [SerializeField] private WorldInteractionDeviceType deviceType;
        [SerializeField] private bool enabledState = true;

        private WorldInteractionModel model;

        public string DeviceId => deviceId;
        public bool IsEnabled => model != null && model.IsEnabled(deviceId);

        public void Initialize(WorldInteractionModel sharedModel)
        {
            model = sharedModel;
            if (model == null || string.IsNullOrWhiteSpace(deviceId))
                return;

            model.Register(deviceId, deviceType, enabledState);
        }

        public bool SetEnabled(bool value)
        {
            if (model == null || string.IsNullOrWhiteSpace(deviceId))
                return false;

            var action = value ? WorldInteractionAction.Enable : WorldInteractionAction.Disable;
            return model.TryApply(deviceId, action);
        }
    }
}
