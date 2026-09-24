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
            Time.timeScale = 0f;
            return true;
        }

        public bool ResumeGame()
        {
            if (!SetState(GameplayState.Playing)) return false;
            Time.timeScale = 1f;
            return true;
        }

        public bool MarkDead()
        {
            if (!SetState(GameplayState.Dead)) return false;
            Time.timeScale = 0f;
            return true;
        }

        public bool RestartPlayer()
        {
            Time.timeScale = 1f;
            playerHealth?.ResetHealth();
            return SetState(GameplayState.Playing);
        }

        bool SetState(GameplayState next)
        {
            var changed = stateService.TrySet(next);
            if (changed && next != GameplayState.Paused && next != GameplayState.Dead)
                Time.timeScale = 1f;
            return changed;
        }

        void OnStateChanged(GameplayState previous, GameplayState next) => StateChanged?.Invoke(previous, next);

        private void OnDestroy()
        {
            stateService.Changed -= OnStateChanged;
            if (Instance == this) { Time.timeScale = 1f; Instance = null; }
        }
    }
}
