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
        public TileMap TileMap { get; }
        public Camera Camera { get; set; }
        public GameBoard Board { get; set; }
        public UiRoot UiRoot { get; set; }
        public PlayerInventory Inventory { get; set; }
        public GameStats Stats { get; } = new GameStats();
        public InputState InputState { get; } = new InputState();
        public HitEmitter HitEmitter { get; set; }
        public System.Random SharedRandom { get; private set; }

        public GameContext(GameConfig config)
        {
            Instance = this;
            Config = config;
            TileMap = new TileMap();
            SharedRandom = new System.Random(config.spawn.randomSeed);
        }

        /// <summary>Test-only constructor — bypasses pool/field setup.</summary>
        internal GameContext(GameConfig config, TileMap tileMap)
        {
            Config = config;
            TileMap = tileMap;
            SharedRandom = new System.Random(config.spawn.randomSeed);
        }
    }
}
