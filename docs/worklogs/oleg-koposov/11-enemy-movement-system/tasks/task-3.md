# Task 3: EnemyMovementSystem + EnemySpawnSystem + HopAnimationSystem + GameplayState

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/EnemyMovementSystem.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/EnemySpawnSystem.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/HopAnimationSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** `11 Add EnemyMovementSystem, generalize HopAnimationSystem, wire up in GameplayState`

### Steps

1. **Modify `EnemySpawnSystem.cs`** — expose active enemies and add a spawn callback:

   a. Change `_activeEnemies` to expose a read-only list:
   ```csharp
   public IReadOnlyList<EnemyView> ActiveEnemies => _activeEnemies;
   ```

   b. Add spawn callback field:
   ```csharp
   /// <summary>Invoked when an enemy is spawned. Subscribers assign per-enemy state.</summary>
   public System.Action<EnemyView> EnemySpawned;
   ```

   c. In `SpawnEnemyAt`, after `_activeEnemies.Add(enemy)`:
   ```csharp
   EnemySpawned?.Invoke(enemy);
   ```

2. **Create `EnemyMovementSystem.cs`** in `Core/Systems/`:

   ```csharp
   using System.Collections.Generic;
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class EnemyMovementSystem : IGameSystem
       {
           private readonly GameContext _context;
           private readonly GridMovementSystem _gridMovement;
           private readonly Dictionary<EnemyView, EnemyMoveState> _states
               = new Dictionary<EnemyView, EnemyMoveState>();
           private readonly Dictionary<EnemyType, IMoveStrategy> _strategies;

           private float _beatTimer;

           public EnemyMovementSystem(GameContext context, GridMovementSystem gridMovement)
           {
               _context = context;
               _gridMovement = gridMovement;
               _strategies = new Dictionary<EnemyType, IMoveStrategy>
               {
                   { EnemyType.Pawn,   new PawnMoveStrategy()   },
                   { EnemyType.Knight, new KnightMoveStrategy() },
                   { EnemyType.Rook,   new RookMoveStrategy()   },
                   { EnemyType.Tower,  new TowerMoveStrategy()  },
                   { EnemyType.Queen,  new QueenMoveStrategy()  },
               };
           }

           public void Initialize() { _beatTimer = 0f; }

           public void Tick(float deltaTime)
           {
               float beatDuration = _context.Config.timing.beatDuration;
               float prevTimer = _beatTimer;
               _beatTimer += deltaTime;

               // Collect keys to avoid modifying dict while iterating
               var keys = new List<EnemyView>(_states.Keys);

               foreach (var enemy in keys)
               {
                   if (!_states.TryGetValue(enemy, out var state)) continue;

                   float triggerTime = state.phaseOffset * beatDuration;
                   if (!CrossedThreshold(prevTimer, _beatTimer, triggerTime, beatDuration)) continue;

                   if (state.cooldownTicksLeft > 0)
                   {
                       state.cooldownTicksLeft--;
                       _states[enemy] = state;
                       continue;
                   }

                   if (!_strategies.TryGetValue(enemy.Config.enemyType, out var strategy)) continue;

                   var dir = strategy.GetMoveDirection(enemy, _context, _context.SharedRandom);
                   if (dir.HasValue)
                   {
                       _gridMovement.TryMove(enemy, dir.Value);
                       state.cooldownTicksLeft = enemy.Config.moveCooldownTicks;
                       _states[enemy] = state;
                   }
               }

               if (_beatTimer >= beatDuration) _beatTimer -= beatDuration;
           }

           public void Dispose() => _states.Clear();

           // ── Public callbacks ────────────────────────────────────────────

           public void OnEnemySpawned(EnemyView enemy)
           {
               _states[enemy] = new EnemyMoveState
               {
                   phaseOffset = (float)_context.SharedRandom.NextDouble(),
                   phaseTimer = 0f,
                   cooldownTicksLeft = 0,
               };
           }

           public void OnEnemyDied(EnemyView enemy) => _states.Remove(enemy);

           // ── Private helpers ─────────────────────────────────────────────

           /// <summary>
           /// Returns true if <paramref name="threshold"/> was crossed in (prevTimer, prevTimer+delta]
           /// accounting for wrapping at beatDuration.
           /// </summary>
           private static bool CrossedThreshold(float prev, float next, float threshold, float period)
           {
               if (next < period)
                   return prev < threshold && next >= threshold;
               // Wrapped
               float wrapped = next - period;
               return prev < threshold || wrapped >= threshold;
           }
       }
   }
   ```

3. **Modify `HopAnimationSystem.cs`** — accept `EnemySpawnSystem` and tick all active enemies:

   ```csharp
   namespace CrimsonBoard
   {
       public class HopAnimationSystem : IGameSystem
       {
           private readonly GameContext _context;
           private readonly EnemySpawnSystem _enemySpawn;

           public HopAnimationSystem(GameContext context, EnemySpawnSystem enemySpawn)
           {
               _context = context;
               _enemySpawn = enemySpawn;
           }

           public void Initialize() { }

           public void Tick(float deltaTime)
           {
               _context.Player?.TickHop(deltaTime);
               if (_enemySpawn == null) return;
               foreach (var enemy in _enemySpawn.ActiveEnemies)
                   enemy.TickHop(deltaTime);
           }

           public void Dispose() { }
       }
   }
   ```

4. **Modify `GameplayState.cs`**:

   a. Add field:
   ```csharp
   private EnemyMovementSystem _enemyMovementSystem;
   ```

   b. In constructor, replace the `HopAnimationSystem` instantiation (it now requires `enemySpawnSystem`):
   ```csharp
   _systemRunner.RegisterSystem(new HopAnimationSystem(context, _enemySpawnSystem));
   ```
   > Previously: `new HopAnimationSystem(context)` — must update to pass `_enemySpawnSystem`.

   c. After registering `_enemySpawnSystem`, create and register `EnemyMovementSystem`:
   ```csharp
   _enemyMovementSystem = new EnemyMovementSystem(context, _gridMovementSystem);
   _systemRunner.RegisterSystem(_enemyMovementSystem);
   ```

   d. Wire all spawn/death callbacks (replace current single assignment with multicast `+=`):
   ```csharp
   _enemySpawnSystem.EnemySpawned += _enemyMovementSystem.OnEnemySpawned;
   _healthSystem.EnemyDeathCallback += _enemySpawnSystem.OnEnemyDied;
   _healthSystem.EnemyDeathCallback += _enemyMovementSystem.OnEnemyDied;
   ```
   > In task 10 we set `_healthSystem.EnemyDeathCallback = _enemySpawnSystem.OnEnemyDied;` — change `=` to `+=` and add the movement system line.

## Implementation
<!-- Filled in Phase 3 -->
