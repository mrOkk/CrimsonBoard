using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrimsonBoard
{
    public class EnemySpawnSystem : IGameSystem
    {
        private readonly GameContext _context;
        private EnemyConfig[] _enemyConfigs;
        private HealthSystem _healthSystem;

        private int _currentWaveIndex;
        private float _waveTimer;
        private float _spawnTimer;

        public System.Action<EnemyView> EnemySpawned;

        public EnemySpawnSystem(GameContext context, HealthSystem healthSystem)
        {
            _context = context;
            _healthSystem = healthSystem;
            _enemyConfigs = new EnemyConfig[_context.Config.enemies.Max(e => e.id) + 1];

            foreach (var cfg in _context.Config.enemies)
            {
                if (_enemyConfigs[cfg.id] != null)
                {
                    Debug.LogWarning($"[EnemySpawnSystem] Duplicate EnemyConfig id={cfg.id}");
                }
                _enemyConfigs[cfg.id] = cfg;
            }
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

            var shuffledIndexes = _context.TileMap.GetShuffledIndexes();
            var playerCell = _context.Player.CurrentCell;

            int spawned = 0;

            for (var i = 0; i < shuffledIndexes.Length && spawned < batch; i++)
            {
                var cellIndex = shuffledIndexes[i];
                var cell = _context.TileMap.IndexToCell(cellIndex);

                if (_context.TileMap.IsOccupied(cell))
                {
                    continue;
                }

                var dist = Vector2Int.Distance(cell, playerCell);
                if (dist < _context.Config.spawn.minDistanceFromPlayer)
                {
                    continue;
                }

                SpawnEnemyAt(cell, wave);
                spawned++;
            }
        }

        private void SpawnEnemyAt(Vector2Int cell, WaveConfig wave)
        {
            var enemyId = PickEnemyId(wave);
            var cfg = _enemyConfigs[enemyId]; // Assume configs are properly populated and indexed by ID; null check below just in case

            if (cfg == null)
            {
                Debug.LogWarning($"[EnemySpawnSystem] No EnemyConfig for id={enemyId}");
                return;
            }

            var enemy = _context.Pools.Enemies.Get();
            enemy.Setup(cfg);
            enemy.CurrentCell = cell;
            enemy.transform.position = _context.TileMap.CellToWorld(cell);
            _context.TileMap.RegisterEntity(cell, enemy);
            _context.Board.RegisterEnemy(enemy);

            enemy.Health.OnDeath += () =>
            {
                if (_healthSystem != null)
                    _healthSystem.OnEnemyDeath(enemy, enemy.CurrentCell);
            };

            EnemySpawned?.Invoke(enemy);
        }

        /// <summary>
        /// Deterministic weighted pick. Pure static for unit-testability.
        /// </summary>
        public static int PickEnemyId(EnemySpawnEntry[] entries, System.Random rng)
        {
            var total = 0f;

            for (var index = 0; index < entries.Length; index++)
            {
                var entry = entries[index];
                total += entry.weight;
            }

            var roll = (float)(rng.NextDouble() * total);
            var acc = 0f;

            for (var index = 0; index < entries.Length; index++)
            {
                var entry = entries[index];
                acc += entry.weight;

                if (roll < acc) return entry.enemyId;
            }

            return entries[^1].enemyId;
        }

        private int PickEnemyId(WaveConfig wave) => PickEnemyId(wave.enemyTypes, _context.SharedRandom);
    }
}
