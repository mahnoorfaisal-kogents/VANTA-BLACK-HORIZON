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
        [SerializeField] private GameSession session;

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
            GUI.Label(new Rect(20, 50, 620, 24), $"HP {hp}   |   AMMO {(weapon ? weapon.AmmoInMagazine : 0)}   |   WANTED {(wanted ? wanted.Level : 0)}", body);
            GUI.Label(new Rect(20, 74, 620, 24), "WASD Move   SHIFT Sprint   CTRL Crouch   SPACE Jump   RMB Aim   LMB Fire   R Reload   F Melee   B Bullet Time   E Interact   F5 Save   F9 Load   ESC Pause", body);
            if (interaction && interaction.CurrentDevice)
                GUI.Label(new Rect(20, 98, 520, 24), $"INTERACT // {interaction.CurrentDevice.DeviceType} [{interaction.CurrentDevice.DeviceId}]", body);
            GUI.Label(new Rect(Screen.width - 250, 18, 230, 24), "VANTA // LIVE DISTRICT", body);
            if (session && session.State == GameplayState.Dead)
            {
                GUI.Label(new Rect(Screen.width * 0.5f - 180f, Screen.height * 0.5f - 35f, 360f, 40f), "YOU ARE DOWN", title);
                GUI.Label(new Rect(Screen.width * 0.5f - 220f, Screen.height * 0.5f + 10f, 440f, 30f), "Press ENTER to restart", body);
            }
        }
    }
}
