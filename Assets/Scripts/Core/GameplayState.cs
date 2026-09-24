namespace Vanta.Core
{
    public enum GameplayState { MainMenu, Playing, Paused, Dead }

    public sealed class GameplayStateService
    {
        public GameplayState Current { get; private set; } = GameplayState.MainMenu;
        public void Set(GameplayState state) => Current = state;
    }
}
