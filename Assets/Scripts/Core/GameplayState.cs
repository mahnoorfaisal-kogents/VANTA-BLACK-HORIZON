using System;

namespace Vanta.Core
{
    public enum GameplayState { MainMenu, Playing, Paused, Dead }

    public sealed class GameplayStateService
    {
        public GameplayState Current { get; private set; } = GameplayState.MainMenu;
        public event Action<GameplayState, GameplayState> Changed;

        public bool TrySet(GameplayState state)
        {
            if (Current == state) return true;
            var allowed = Current switch
            {
                GameplayState.MainMenu => state == GameplayState.Playing,
                GameplayState.Playing => state == GameplayState.Paused || state == GameplayState.Dead || state == GameplayState.MainMenu,
                GameplayState.Paused => state == GameplayState.Playing || state == GameplayState.MainMenu,
                GameplayState.Dead => state == GameplayState.Playing || state == GameplayState.MainMenu,
                _ => false
            };
            if (!allowed) return false;
            var previous = Current;
            Current = state;
            Changed?.Invoke(previous, state);
            return true;
        }

        public void Reset() => Current = GameplayState.MainMenu;
    }
}
