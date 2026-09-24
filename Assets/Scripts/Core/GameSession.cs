using UnityEngine;
using Vanta.Player;

namespace Vanta.Core
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }
        public GameplayState State { get; private set; } = GameplayState.MainMenu;
        [SerializeField] private Health playerHealth;

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartGame() => State = GameplayState.Playing;
        public void PauseGame() { if (State == GameplayState.Playing) { State = GameplayState.Paused; Time.timeScale = 0f; } }
        public void ResumeGame() { if (State == GameplayState.Paused) { State = GameplayState.Playing; Time.timeScale = 1f; } }
        public void MarkDead() { State = GameplayState.Dead; Time.timeScale = 0f; }
        public void RestartPlayer()
        {
            Time.timeScale = 1f;
            playerHealth?.ResetHealth();
            State = GameplayState.Playing;
        }

        private void OnDestroy() { if (Instance == this) { Time.timeScale = 1f; Instance = null; } }
    }
}
