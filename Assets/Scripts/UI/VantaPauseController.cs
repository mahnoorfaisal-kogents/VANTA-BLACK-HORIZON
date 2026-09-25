using UnityEngine;
using Vanta.Core;

namespace Vanta.UI
{
    public sealed class VantaPauseController : MonoBehaviour
    {
        [SerializeField] GameSession session;
        public bool IsPaused => session && session.State == GameplayState.Paused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                HandlePauseInput(KeyCode.Escape);
            if (Input.GetKeyDown(KeyCode.Return))
                HandleRestartInput(KeyCode.Return);
        }

        public void Configure(GameSession gameSession) => session = gameSession;

        public bool HandlePauseInput(KeyCode key)
        {
            if (key != KeyCode.Escape || !session) return false;
            TogglePause();
            return true;
        }

        public bool HandleRestartInput(KeyCode key)
        {
            if (key != KeyCode.Return || !session || session.State != GameplayState.Dead)
                return false;
            return session.RestartPlayer();
        }

        public void TogglePause()
        {
            if (!session) return;
            if (IsPaused) session.ResumeGame();
            else if (session.State == GameplayState.Playing) session.PauseGame();
        }
    }
}
