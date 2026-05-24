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
        public GamePools Pools { get; set; }
        public PlayerView Player { get; set; }

        public GameContext(GameConfig config)
        {
            Instance = this;
            Config = config;
        }
    }
}
