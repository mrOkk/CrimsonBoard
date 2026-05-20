namespace CrimsonBoard
{
    /// <summary>
    /// Central dependency container. Passed to every game state.
    /// Add typed fields as systems are implemented in future tasks.
    /// </summary>
    public class GameContext
    {
        public static GameContext Instance { get; private set; }

        public GameConfig Config { get; }

        public GameContext(GameConfig config)
        {
            Instance = this;
            Config = config;
        }
    }
}
