using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class EnemySpawnSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly GameFieldSystem _gameFieldSystem;

        private int _currentWaveIndex;
        private float _waveTimer;
        private float _spawnTimer;

        public System.Action<EnemyView> EnemySpawned;

        public EnemySpawnSystem(GameContext context, GameFieldSystem gameFieldSystem)
        {
            _context = context;
            _gameFieldSystem = gameFieldSystem;
        }

        public void Initialize()
        {
            _currentWaveIndex = 0;
            _waveTimer = 0f;
            _spawnTimer = NextSpawnInterval();
        }

        public void Tick(float deltaTime)
        {
            var spawnCfg = _context.Config.spawn;
            if (spawnCfg.waves == null || spawnCfg.waves.Length == 0) return;

            // Advance wave timer (clamp at last wave)
            if (_currentWaveIndex < spawnCfg.waves.Length - 1)
            {
                _waveTimer += deltaTime;
                if (_waveTimer >= spawnCfg.waveInterval)
                {
                    _waveTimer -= spawnCfg.waveInterval;
                    _currentWaveIndex++;
                    _spawnTimer = NextSpawnInterval();
                    Debug.Log($"[EnemySpawnSystem] Wave → {_currentWaveIndex}");
                }
            }

            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnBatch();
                _spawnTimer = NextSpawnInterval();
            }
        }

        public void Dispose() { }

        // ── Public callbacks ────────────────────────────────────────────────

        /// <summary>Called by HealthSystem when an enemy's HP reaches zero.</summary>
        public void OnEnemyDied(EnemyView enemy)
        {
            _context.Board.UnregisterEnemy(enemy);
        }

        // ── Private helpers ─────────────────────────────────────────────────

        private WaveConfig CurrentWave => _context.Config.spawn.waves[_currentWaveIndex];

        private float NextSpawnInterval()
        {
            var range = CurrentWave.spawnFrequencyRangeSec;
            float t = (float)_context.SharedRandom.NextDouble();
            return Mathf.Lerp(range.x, range.y, t);
        }

        private void SpawnBatch()
        {
            var wave = CurrentWave;

            int alive = _context.Board.ActiveEnemies.Count;
            if (alive >= wave.maxAliveEnemies) return;

            var batchRange = wave.spawnBatchSizeRange;
            int batch = _context.SharedRandom.Next(batchRange.x, batchRange.y + 1);
            batch = Mathf.Min(batch, wave.maxAliveEnemies - alive);

            var border = _gameFieldSystem.GetBorderTiles();
            Shuffle(border);

            int spawned = 0;
            foreach (var cell in border)
            {
                if (spawned >= batch) break;
                if (_context.OccupancyMap.IsOccupied(cell)) continue;

                SpawnEnemyAt(cell, wave);
                spawned++;
            }
        }

        private void SpawnEnemyAt(Vector2Int cell, WaveConfig wave)
        {
            int enemyId = PickEnemyId(wave);
            var cfg = System.Array.Find(_context.Config.enemies, e => e.id == enemyId);
            if (cfg == null)
            {
                Debug.LogWarning($"[EnemySpawnSystem] No EnemyConfig for id={enemyId}");
                return;
            }

            var enemy = _context.Pools.Enemies.Get();
            enemy.Setup(cfg);
            enemy.CurrentCell = cell;
            enemy.transform.position = ChunkCoordConverter.TileToWorld(cell, _context.Config.board);
            _context.OccupancyMap.Register(cell, enemy);
            _context.Board.RegisterEnemy(enemy);
            EnemySpawned?.Invoke(enemy);
        }

        /// <summary>
        /// Deterministic weighted pick. Pure static for unit-testability.
        /// </summary>
        public static int PickEnemyId(EnemySpawnEntry[] entries, System.Random rng)
        {
            float total = 0f;
            foreach (var entry in entries) total += entry.weight;
            float roll = (float)(rng.NextDouble() * total);
            float acc = 0f;
            foreach (var entry in entries)
            {
                acc += entry.weight;
                if (roll < acc) return entry.enemyId;
            }
            return entries[entries.Length - 1].enemyId;
        }

        private int PickEnemyId(WaveConfig wave) => PickEnemyId(wave.enemyTypes, _context.SharedRandom);

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _context.SharedRandom.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
