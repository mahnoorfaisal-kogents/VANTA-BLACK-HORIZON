using System;
using UnityEngine;
using Vanta.Player;

namespace Vanta.Core
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }
        public GameplayState State => stateService.Current;
        public event Action<GameplayState, GameplayState> StateChanged;
        [SerializeField] private Health playerHealth;
        readonly GameplayStateService stateService = new();

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            stateService.Changed += OnStateChanged;
            DontDestroyOnLoad(gameObject);
        }

        public bool StartGame() => SetState(GameplayState.Playing);

        public bool PauseGame()
        {
            if (!SetState(GameplayState.Paused)) return false;
            return true;
        }

        public bool ResumeGame()
        {
            return SetState(GameplayState.Playing);
        }

        public bool MarkDead()
        {
            return SetState(GameplayState.Dead);
        }

        public bool RestartPlayer()
        {
            Time.timeScale = 1f;
            playerHealth?.ResetHealth();
            return SetState(GameplayState.Playing);
        }

        bool SetState(GameplayState next) => stateService.TrySet(next);

        void OnStateChanged(GameplayState previous, GameplayState next)
        {
            Time.timeScale = next == GameplayState.Paused || next == GameplayState.Dead ? 0f : 1f;
            StateChanged?.Invoke(previous, next);
        }

        private void OnDestroy()
        {
            stateService.Changed -= OnStateChanged;
            if (Instance == this) { Time.timeScale = 1f; Instance = null; }
        }
    }
}
