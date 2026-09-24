using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vanta.UI
{
    public sealed class VantaMainMenu : MonoBehaviour
    {
        private GUIStyle title;
        private GUIStyle body;

        private void Awake()
        {
            title = new GUIStyle(GUI.skin.label) { fontSize = 32, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            body = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
        }

        private void OnGUI()
        {
            if (title == null) Awake();

            var center = Screen.width * 0.5f;
            GUI.Label(new Rect(center - 300f, 100f, 600f, 60f), "VANTA: BLACK HORIZON", title);
            GUI.Label(new Rect(center - 300f, 165f, 600f, 30f), "ORIGINAL OPEN-WORLD ACTION // CORE PLAYABLE BUILD", body);

            if (GUI.Button(new Rect(center - 120f, 240f, 240f, 52f), "NEW GAME"))
                SceneManager.LoadScene("PlayableDistrict");

            if (GUI.Button(new Rect(center - 120f, 305f, 240f, 42f), "QUIT"))
                Application.Quit();

            GUI.Label(new Rect(center - 300f, Screen.height - 70f, 600f, 30f),
                "Original IP • procedural test district • systems-first vertical slice", body);
        }
    }
}
