using UnityEngine;

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
        public OccupancyMap OccupancyMap { get; }
        public Camera Camera { get; set; }
        public GameFieldSystem GameFieldSystem { get; set; }
        public EnemySpawnSystem EnemySpawnSystem { get; set; }
        public UiRoot UiRoot { get; set; }
        public GameStats Stats { get; } = new GameStats();
        public InputState InputState { get; } = new InputState();
        public System.Random SharedRandom { get; private set; }

        public GameContext(GameConfig config)
        {
            Instance = this;
            Config = config;
            OccupancyMap = new OccupancyMap();
            SharedRandom = new System.Random(config.spawn.randomSeed);
        }

        /// <summary>Test-only constructor — bypasses pool/field setup.</summary>
        internal GameContext(GameConfig config, OccupancyMap occupancyMap)
        {
            Config = config;
            OccupancyMap = occupancyMap;
            SharedRandom = new System.Random(config.spawn.randomSeed);
        }
    }
}
