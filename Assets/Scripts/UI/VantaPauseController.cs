using UnityEngine;
using Vanta.Core;

namespace Vanta.UI
{
    public sealed class VantaPauseController : MonoBehaviour
    {
        [SerializeField] GameSession session;
        public bool IsPaused => session && session.State == GameplayState.Paused;

        public void TogglePause()
        {
            if (!session) return;
            if (IsPaused) session.ResumeGame();
            else if (session.State == GameplayState.Playing) session.PauseGame();
        }
    }
}