# Task 4: Edit Mode Tests

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Tests/Editor/EnemyMovementTests.cs`

**Commit message:** `11 Add Edit Mode tests for enemy movement strategies`

### Steps

1. **Add `EnemyMovementTests.cs`** to the existing `Tests/Editor/` folder (asmdef already created in task 10).

   The test file needs a minimal `GameContext`-free setup. Since all strategies are stateless and `IMoveStrategy.GetMoveDirection` takes `GameContext`, we need a stub. Use a minimal approach: create a helper that builds a minimal context from scratch (no Unity objects) using a `MockOccupancyMap` or bypass, OR — extract a pure static helper from each strategy.

   **Better approach:** Test through a thin wrapper that passes in only what the strategy needs. Looking at the interface, strategies need:
   - `enemy.CurrentCell` (Vector2Int — no Unity, just set directly on EnemyView... but EnemyView is MonoBehaviour)
   - `ctx.Player.CurrentCell`
   - `ctx.OccupancyMap`
   - `ctx.SharedRandom`

   Since `EnemyView` and `PlayerView` are MonoBehaviours, we can't instantiate them in Edit Mode without `GameObject`. Use `new GameObject().AddComponent<EnemyView>()` which is allowed in Edit Mode (NUnit tears down after each test via `[TearDown]`).

   **Test cases:**

   **A. Pawn — moves toward player:**
   - Setup: enemy at (0,0), player at (3,0), all cells free
   - Expected: strategy returns `(1,0)` (east, toward player)

   **B. Pawn — avoids occupied cell:**
   - Setup: enemy at (0,0), player at (3,0), cell (1,0) occupied by another enemy
   - Expected: returns a direction other than `(1,0)`, or null if all 4 blocked

   **C. Knight — L-shape to player:**
   - Setup: enemy at (0,0), player at (2,1), all cells free
   - Expected: strategy returns `(2,1)` offset (direct L-jump)

   **D. Knight — early landing on player in path:**
   - Setup: enemy at (0,0), L-move target `(2,1)`, player at (1,0) (intermediate cell)
   - Expected: strategy returns direction toward (1,0) → `(1,0)`

   **E. Knight — higher-rank enemy at target, fallback to intermediate:**
   - Setup: enemy (rank=1) at (0,0), target (2,1) occupied by enemy rank=3
   - Expected: fallback to last free intermediate cell

   **F. Rook — picks diagonal toward player:**
   - Setup: enemy at (0,0), player at (3,3), all free
   - Expected: returns `(1,1)` (diagonal toward player)

   **G. Tower — picks cardinal toward player:**
   - Setup: enemy at (0,0), player at (0,4), all free
   - Expected: returns `(0,1)` (north, toward player)

   **H. Queen — picks direction minimising distance:**
   - Setup: enemy at (0,0), player at (3,2), all free
   - Expected: one of the 8 directions; verify returned dir moves enemy closer to player

   **I. LinearStrategy — stops at first blocked cell:**
   - Setup: enemy at (0,0), direction (1,0), cells (2,0) occupied
   - Rook/Tower walks from (0,0): can reach (1,0) but not (2,0) → `reached = (1,0)`
   - Verify returned direction is `(1,0)` (still valid, just shorter reach)

   ```csharp
   using NUnit.Framework;
   using UnityEngine;
   using CrimsonBoard;

   namespace CrimsonBoard.Tests
   {
       public class EnemyMovementTests
       {
           private GameObject _enemyGo, _playerGo, _blockerGo;
           private EnemyView _enemy;
           private PlayerView _player;

           [SetUp]
           public void SetUp()
           {
               _enemyGo  = new GameObject("Enemy");
               _playerGo = new GameObject("Player");
               _enemy  = _enemyGo.AddComponent<EnemyView>();
               _player = _playerGo.AddComponent<PlayerView>();
           }

           [TearDown]
           public void TearDown()
           {
               Object.DestroyImmediate(_enemyGo);
               Object.DestroyImmediate(_playerGo);
               if (_blockerGo != null) Object.DestroyImmediate(_blockerGo);
           }

           private (GameContext ctx, EnemyConfig cfg) MakeContext(
               Vector2Int enemyCell, Vector2Int playerCell,
               int enemyRank = 1, EnemyType enemyType = EnemyType.Pawn)
           {
               var cfg = new EnemyConfig { id = 1, rank = enemyRank, enemyType = enemyType,
                   health = 1, damage = 1, movesPerBeat = 1, moveCooldownTicks = 0 };
               _enemy.CurrentCell = enemyCell;
               _player.CurrentCell = playerCell;

               var map = new OccupancyMap();
               map.Register(playerCell, _player);

               // Build a minimal context via reflection to avoid ScriptableObject dependency
               // Use the internal ctor overload that accepts only what we need
               var ctx = new TestGameContext(map, _player, new System.Random(0));
               return (ctx, cfg);
           }

           // ── Pawn ──────────────────────────────────────────────────────────

           [Test]
           public void Pawn_MovesTowardPlayer()
           {
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(3, 0);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               var strategy = new PawnMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               Assert.AreEqual(new Vector2Int(1, 0), dir, "Pawn should move east toward player");
           }

           [Test]
           public void Pawn_AvoidsOccupiedCell()
           {
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(3, 0);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               // Block east
               _blockerGo = new GameObject("Blocker");
               var blocker = _blockerGo.AddComponent<EnemyView>();
               blocker.CurrentCell = new Vector2Int(1, 0);
               map.Register(blocker.CurrentCell, blocker);

               var strategy = new PawnMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               Assert.AreNotEqual(new Vector2Int(1, 0), dir, "Pawn should not move into occupied cell");
           }

           // ── Knight ────────────────────────────────────────────────────────

           [Test]
           public void Knight_LJumpToPlayer()
           {
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(2, 1);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               var strategy = new KnightMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               Assert.AreEqual(new Vector2Int(2, 1), dir, "Knight should pick L-jump directly to player");
           }

           [Test]
           public void Knight_EarlyLandingOnPlayerInPath()
           {
               // Enemy at (0,0), target would be (2,1); player at (1,0) = intermediate
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(1, 0);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               var strategy = new KnightMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               Assert.IsNotNull(dir);
               // Should step toward (1,0)
               Assert.AreEqual(new Vector2Int(1, 0), dir);
           }

           // ── Rook (diagonal) ───────────────────────────────────────────────

           [Test]
           public void Rook_PicksDiagonalTowardPlayer()
           {
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(3, 3);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               var strategy = new RookMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               Assert.AreEqual(new Vector2Int(1, 1), dir, "Rook should pick NE diagonal toward player");
           }

           [Test]
           public void Rook_StopsAtBlockedCell()
           {
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(5, 5);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               // Block NE at (2,2)
               _blockerGo = new GameObject("Blocker");
               var blocker = _blockerGo.AddComponent<EnemyView>();
               blocker.CurrentCell = new Vector2Int(2, 2);
               map.Register(blocker.CurrentCell, blocker);

               var strategy = new RookMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               // Still picks NE (1,1), reaches only (1,1) before blocker at (2,2)
               Assert.AreEqual(new Vector2Int(1, 1), dir);
           }

           // ── Tower (straight) ──────────────────────────────────────────────

           [Test]
           public void Tower_PicksCardinalTowardPlayer()
           {
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(0, 4);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               var strategy = new TowerMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               Assert.AreEqual(new Vector2Int(0, 1), dir, "Tower should move north toward player");
           }

           // ── Queen ─────────────────────────────────────────────────────────

           [Test]
           public void Queen_PicksDirectionClosestToPlayer()
           {
               _enemy.CurrentCell = Vector2Int.zero;
               _player.CurrentCell = new Vector2Int(4, 0);
               var map = new OccupancyMap();
               map.Register(_player.CurrentCell, _player);

               var strategy = new QueenMoveStrategy();
               var dir = strategy.GetMoveDirection(_enemy, MakeTestCtx(map), new System.Random(0));

               Assert.AreEqual(new Vector2Int(1, 0), dir, "Queen should move east toward player");
           }

           // ── Helper ────────────────────────────────────────────────────────

           private GameContext MakeTestCtx(OccupancyMap map)
               => TestGameContext.Create(map, _player, new System.Random(0));
       }
   }
   ```

   > **`TestGameContext`** is a test-only helper that creates a minimal `GameContext` without requiring `GameConfig` ScriptableObject. Add it as a nested class or separate file in `Tests/Editor/`:
   ```csharp
   // TestGameContext.cs in Tests/Editor/
   namespace CrimsonBoard.Tests
   {
       internal static class TestGameContext
       {
           internal static GameContext Create(OccupancyMap map, PlayerView player, System.Random rng)
           {
               // GameContext ctor requires GameConfig. We use reflection to bypass and
               // inject the fields we need directly, since GameConfig is a ScriptableObject.
               // Alternative: add an internal test-only constructor to GameContext.
               // Recommended: add internal ctor to GameContext (see note below).
               throw new System.NotImplementedException("See implementation note");
           }
       }
   }
   ```
   > **Implementation note:** Since `GameContext` requires `GameConfig` (ScriptableObject), we have two options:
   > 1. Add a second constructor to `GameContext` for testing: `internal GameContext(OccupancyMap map, PlayerView player, System.Random rng)` — sets only what strategies need.
   > 2. Create `ScriptableObject.CreateInstance<GameConfig>()` — works in Edit Mode.
   >
   > **Use option 2 (no extra constructor needed):**
   > ```csharp
   > var cfg = ScriptableObject.CreateInstance<GameConfig>();
   > cfg.board = new BoardConfig(); // default values
   > cfg.spawn = new SpawnConfig();
   > var ctx = new GameContext(cfg);
   > ctx.OccupancyMap = map; // OccupancyMap is readonly — need to expose setter or use reflection
   > ```
   > Since `OccupancyMap` is read-only in `GameContext`, the cleanest fix is to pass the map through the constructor. **Add an overload:**
   > In `GameContext.cs`:
   > ```csharp
   > /// <summary>Test-only constructor.</summary>
   > internal GameContext(GameConfig config, OccupancyMap occupancyMap)
   > {
   >     Config = config;
   >     OccupancyMap = occupancyMap;
   >     SharedRandom = new System.Random(config.spawn.randomSeed);
   > }
   > ```
   > Then tests create context as:
   > ```csharp
   > var cfg = ScriptableObject.CreateInstance<GameConfig>();
   > cfg.spawn = new SpawnConfig { randomSeed = 0 };
   > var ctx = new GameContext(cfg, map);
   > ctx.Player = player;
   > ```

2. **Add internal `GameContext` test constructor** in `GameContext.cs`:
   ```csharp
   internal GameContext(GameConfig config, OccupancyMap occupancyMap)
   {
       Config = config;
       OccupancyMap = occupancyMap;
       SharedRandom = new System.Random(config.spawn.randomSeed);
   }
   ```

3. **Replace `TestGameContext` stub** with real implementation using `ScriptableObject.CreateInstance`:
   ```csharp
   internal static GameContext Create(OccupancyMap map, PlayerView player, System.Random rng)
   {
       var cfg = ScriptableObject.CreateInstance<GameConfig>();
       cfg.spawn = new SpawnConfig { randomSeed = 0 };
       var ctx = new GameContext(cfg, map);
       ctx.Player = player;
       return ctx;
   }
   ```

4. Add `[TearDown]` destroy for `ScriptableObject.DestroyImmediate(cfg)` to avoid memory leaks between tests.

## Implementation
**Status:** DONE
**Summary:** Created `EnemyMovementTests.cs` with 11 tests covering all 5 strategies, LinearStrategy blocking, and beat threshold logic. Uses `ScriptableObject.CreateInstance<GameConfig>()` + internal `GameContext` test constructor; sets `EnemyView._config` via reflection to avoid `HealthComponent` NPE in Edit Mode.
