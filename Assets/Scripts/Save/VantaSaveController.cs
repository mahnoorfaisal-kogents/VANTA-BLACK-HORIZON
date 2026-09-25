using UnityEngine;

namespace Vanta.Save
{
    public enum VantaSaveAction { None, Save, Load }

    public sealed class VantaSaveController : MonoBehaviour
    {
        [SerializeField] private SaveGameCoordinator coordinator;
        [SerializeField] private string slot = "slot_01";
        [SerializeField] private KeyCode saveKey = KeyCode.F5;
        [SerializeField] private KeyCode loadKey = KeyCode.F9;

        public void Configure(SaveGameCoordinator saveCoordinator, string saveSlot)
        {
            coordinator = saveCoordinator;
            slot = string.IsNullOrWhiteSpace(saveSlot) ? "slot_01" : saveSlot.Trim();
        }

        public VantaSaveAction ResolveAction(KeyCode key)
        {
            if (key == saveKey) return VantaSaveAction.Save;
            if (key == loadKey) return VantaSaveAction.Load;
            return VantaSaveAction.None;
        }

        private void Update()
        {
            if (!coordinator) return;

            var action = ResolveAction(
                Input.GetKeyDown(saveKey) ? saveKey :
                Input.GetKeyDown(loadKey) ? loadKey :
                KeyCode.None);

            if (action == VantaSaveAction.Save)
                coordinator.SaveSlot(slot);
            else if (action == VantaSaveAction.Load)
                coordinator.LoadSlot(slot);
        }
    }
}
