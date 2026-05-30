# Task 3: EnemySpawnSystem

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/EnemySpawnSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** `10 Add EnemySpawnSystem and register in GameplayState`

### Steps

1. **Create `EnemySpawnSystem.cs`** in `Core/Systems/`. The system must:
   - Hold `_currentWaveIndex`, `_waveTimer` (counts up to `waveInterval`), `_spawnTimer` (counts down to next spawn batch)
   - On `Initialize()`: reset timers, pick initial spawn interval from current wave's `spawnFrequencyRangeSec` range
   - On `Tick(float deltaTime)`: advance both timers; spawn when `_spawnTimer <= 0`; advance wave when `_waveTimer >= waveInterval` (clamp at last wave)

   ```csharp
   using System.Collections.Generic;
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class EnemySpawnSystem : IGameSystem
       {
           private readonly GameContext _context;

           private int _currentWaveIndex;
           private float _waveTimer;
           private float _spawnTimer;

           public EnemySpawnSystem(GameContext context)
           {
               _context = context;
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

               // Advance wave timer (stop at last wave)
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

               // Spawn tick
               _spawnTimer -= deltaTime;
               if (_spawnTimer <= 0f)
               {
                   SpawnBatch();
                   _spawnTimer = NextSpawnInterval();
               }
           }

           public void Dispose()
           {
               // Return all active enemies to pool
               // Note: HealthSystem.OnEnemyDeath handles individual returns;
               // full pool reset on game-over is handled by GamePools re-creation in Init.
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

               // Check alive enemy cap
               int alive = CountAliveEnemies();
               if (alive >= wave.maxAliveEnemies) return;

               // Determine batch size
               var batchRange = wave.spawnBatchSizeRange;
               int batch = _context.SharedRandom.Next(batchRange.x, batchRange.y + 1);
               batch = Mathf.Min(batch, wave.maxAliveEnemies - alive);

               // Get shuffled border tiles
               var border = _context.GameFieldSystem.GetBorderTiles();
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

           private int CountAliveEnemies()
           {
               // Enemies are tracked by OccupancyMap but we need an enemy-specific count.
               // Keep an explicit counter via events from HealthSystem.
               // For iteration 1: walk active enemies list maintained below.
               return _activeEnemies.Count;
           }

           private readonly List<EnemyView> _activeEnemies = new List<EnemyView>();

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
               _activeEnemies.Add(enemy);
           }

           /// <summary>Call from HealthSystem.OnEnemyDeath before returning to pool.</summary>
           public void OnEnemyDied(EnemyView enemy)
           {
               _activeEnemies.Remove(enemy);
           }

           private int PickEnemyId(WaveConfig wave)
           {
               float total = 0f;
               foreach (var entry in wave.enemyTypes) total += entry.weight;
               float roll = (float)(_context.SharedRandom.NextDouble() * total);
               float acc = 0f;
               foreach (var entry in wave.enemyTypes)
               {
                   acc += entry.weight;
                   if (roll < acc) return entry.enemyId;
               }
               return wave.enemyTypes[wave.enemyTypes.Length - 1].enemyId;
           }

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
   ```

2. **Modify `GameplayState.cs`** — instantiate and register `EnemySpawnSystem` after existing systems. Also wire up `OnEnemyDied` in `HealthSystem.OnEnemyDeath`:
   - Add field: `private EnemySpawnSystem _enemySpawnSystem;`
   - In constructor, after registering `HopAnimationSystem`:
     ```csharp
     _enemySpawnSystem = new EnemySpawnSystem(context);
     _systemRunner.RegisterSystem(_enemySpawnSystem);
     ```
   - Also update `HealthSystem.OnEnemyDeath` to call `_enemySpawnSystem.OnEnemyDied(enemy)` — but since `HealthSystem` doesn't hold a ref to `EnemySpawnSystem`, use a callback approach: expose `System.Action<EnemyView> OnEnemyDied` event on `EnemySpawnSystem` and subscribe in `GameplayState`, **OR** pass `EnemySpawnSystem` to `HealthSystem`.
   
   Simplest: add a `public event System.Action<EnemyView> EnemyDied;` to `EnemySpawnSystem` and subscribe from `GameplayState`:
   ```csharp
   // In GameplayState constructor, after creating _enemySpawnSystem:
   _healthSystem.EnemyDeathCallback = _enemySpawnSystem.OnEnemyDied;
   ```
   Add `public System.Action<EnemyView> EnemyDeathCallback;` to `HealthSystem` and call it in `OnEnemyDeath`:
   ```csharp
   EnemyDeathCallback?.Invoke(enemy);
   ```

## Implementation
**Status:** DONE
**Summary:** Создан `EnemySpawnSystem` с волновыми таймерами, батч-спавном на граничных тайлах и `PickEnemyId` static для тестов. В `HealthSystem` добавлен `EnemyDeathCallback`. В `GameplayState` — регистрация системы и подключение колбека.
