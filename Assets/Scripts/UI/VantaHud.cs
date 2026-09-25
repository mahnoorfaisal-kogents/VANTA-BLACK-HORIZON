using UnityEngine;
using Vanta.Core;
using Vanta.Combat;
using Vanta.Systems;
using Vanta.Player;

namespace Vanta.UI
{
    public sealed class VantaHud : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private string districtName = "BLACK HORIZON // SECTOR 01";
        [SerializeField] private WeaponController weapon;
        [SerializeField] private WantedSystem wanted;
        [SerializeField] private WorldInteractionInteractor interaction;

        private GUIStyle title;
        private GUIStyle body;

        private void Awake()
        {
            title = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            body = new GUIStyle(GUI.skin.label) { fontSize = 14 };
        }

        private void OnGUI()
        {
            if (title == null) Awake();

            GUI.Label(new Rect(20, 18, 520, 30), districtName, title);
            var hp = playerHealth ? Mathf.CeilToInt(playerHealth.CurrentHealth) : 0;
            GUI.Label(new Rect(20, 50, 360, 24), $"HP {hp}   |   WASD Move   SHIFT Sprint   CTRL Crouch", body);
            GUI.Label(new Rect(20, 74, 520, 24), "SPACE Jump   RMB Aim   LMB Fire   R Reload   ESC Pause", body);
            GUI.Label(new Rect(Screen.width - 250, 18, 230, 24), "VANTA // LIVE DISTRICT", body);
        }
    }
}
