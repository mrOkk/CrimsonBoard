# Task 2: IMoveStrategy + EnemyMoveState + 5 movement strategies

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/EnemyMoveState.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/Movement/IMoveStrategy.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/Movement/PawnMoveStrategy.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/Movement/KnightMoveStrategy.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/Movement/RookMoveStrategy.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/Movement/TowerMoveStrategy.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/Movement/QueenMoveStrategy.cs`

**Commit message:** `11 Add IMoveStrategy interface and five enemy movement strategies`

### Steps

1. **Create `EnemyMoveState.cs`** in `Core/Systems/`:
   ```csharp
   namespace CrimsonBoard
   {
       public struct EnemyMoveState
       {
           public float phaseOffset;       // 0..1 — when in the beat cycle to fire
           public float phaseTimer;        // current position in beat cycle (seconds)
           public int cooldownTicksLeft;   // beats remaining before allowed to move
       }
   }
   ```

2. **Create `IMoveStrategy.cs`** in `Core/Systems/Movement/`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       /// <summary>
       /// Returns a direction vector (not an absolute cell) to pass to GridMovementSystem.TryMove.
       /// Returns null if no valid move exists.
       /// </summary>
       public interface IMoveStrategy
       {
           Vector2Int? GetMoveDirection(EnemyView enemy, GameContext context, System.Random rng);
       }
   }
   ```
   Returns a **direction** (e.g. `(1,0)`) rather than an absolute cell — consistent with `GridMovementSystem.TryMove(entity, dir)`.

3. **Create `PawnMoveStrategy.cs`** in `Core/Systems/Movement/`:
   - 4 cardinal directions: `(1,0), (-1,0), (0,1), (0,-1)`
   - Filter: keep only directions where target cell is free OR target cell is the player
   - Sort remaining by Manhattan distance of (enemy.CurrentCell + dir) to player (ascending)
   - Return first, or null if none
   ```csharp
   using System.Collections.Generic;
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class PawnMoveStrategy : IMoveStrategy
       {
           private static readonly Vector2Int[] Cardinals = {
               new Vector2Int(1, 0), new Vector2Int(-1, 0),
               new Vector2Int(0, 1), new Vector2Int(0, -1),
           };

           public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
           {
               var playerCell = ctx.Player?.CurrentCell ?? Vector2Int.zero;
               Vector2Int? best = null;
               int bestDist = int.MaxValue;

               foreach (var dir in Cardinals)
               {
                   var target = enemy.CurrentCell + dir;
                   var occupant = ctx.OccupancyMap.GetEntity(target);
                   bool passable = occupant == null || occupant is PlayerView;
                   if (!passable) continue;

                   int dist = Mathf.Abs(target.x - playerCell.x) + Mathf.Abs(target.y - playerCell.y);
                   if (dist < bestDist)
                   {
                       bestDist = dist;
                       best = dir;
                   }
               }
               return best;
           }
       }
   }
   ```

4. **Create `KnightMoveStrategy.cs`** in `Core/Systems/Movement/`:
   - 8 L-shape offsets: `(±1,±2)` and `(±2,±1)`
   - For each candidate target cell:
     - Empty → valid
     - Player → valid (combat via GridMovementSystem)
     - Enemy with lower rank → valid (crush)
     - Enemy with equal or higher rank → skip
   - Early landing rules (check before picking final target):
     - Define intermediate cells for each L-path
     - If player is at any intermediate cell → return direction toward that intermediate cell (player trampled)
     - If best target has higher-rank enemy → fall back to last free intermediate cell on that L-path
   - Among valid targets, prefer one closest to player
   - Return single-step direction toward chosen target cell (not the full L-jump; `TryMove` is step-based, so pass the L-offset as a delta — **note:** `GridMovementSystem.TryMove` adds dir to CurrentCell, so pass the full L-offset as dir)

   ```csharp
   using System.Collections.Generic;
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class KnightMoveStrategy : IMoveStrategy
       {
           private static readonly Vector2Int[] LMoves = {
               new Vector2Int(1, 2),  new Vector2Int(-1, 2),
               new Vector2Int(1, -2), new Vector2Int(-1, -2),
               new Vector2Int(2, 1),  new Vector2Int(-2, 1),
               new Vector2Int(2, -1), new Vector2Int(-2, -1),
           };

           public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
           {
               var playerCell = ctx.Player?.CurrentCell ?? Vector2Int.zero;
               int myRank = enemy.Config.rank;

               // Check if player is on any intermediate cell → immediate early land
               foreach (var lMove in LMoves)
               {
                   var intermediates = GetIntermediateCells(enemy.CurrentCell, lMove);
                   foreach (var iCell in intermediates)
                   {
                       if (iCell == playerCell)
                           return GetStepToward(enemy.CurrentCell, iCell);
                   }
               }

               // Normal target selection
               Vector2Int? best = null;
               int bestDist = int.MaxValue;

               foreach (var lMove in LMoves)
               {
                   var target = enemy.CurrentCell + lMove;
                   var occupant = ctx.OccupancyMap.GetEntity(target);

                   if (occupant == null || occupant is PlayerView)
                   {
                       // Valid target
                   }
                   else if (occupant is EnemyView otherEnemy && otherEnemy.Config.rank < myRank)
                   {
                       // Can crush lower-rank enemy
                   }
                   else
                   {
                       // Higher or equal rank — fall back to last free intermediate cell
                       var fallback = GetLastFreeIntermediate(enemy.CurrentCell, lMove, ctx.OccupancyMap);
                       if (fallback.HasValue)
                       {
                           int fd = Manhattan(fallback.Value, playerCell);
                           if (fd < bestDist) { bestDist = fd; best = GetStepToward(enemy.CurrentCell, fallback.Value); }
                       }
                       continue;
                   }

                   int dist = Manhattan(target, playerCell);
                   if (dist < bestDist) { bestDist = dist; best = lMove; }
               }
               return best;
           }

           private static List<Vector2Int> GetIntermediateCells(Vector2Int from, Vector2Int lMove)
           {
               int ax = Mathf.Abs(lMove.x), ay = Mathf.Abs(lMove.y);
               int sx = System.Math.Sign(lMove.x), sy = System.Math.Sign(lMove.y);
               var cells = new List<Vector2Int>();

               if (ax == 2) // move 2 in x, 1 in y
               {
                   cells.Add(from + new Vector2Int(sx, 0));
                   cells.Add(from + new Vector2Int(2 * sx, 0));
                   cells.Add(from + new Vector2Int(2 * sx, sy));
               }
               else // move 1 in x, 2 in y
               {
                   cells.Add(from + new Vector2Int(0, sy));
                   cells.Add(from + new Vector2Int(0, 2 * sy));
                   cells.Add(from + new Vector2Int(sx, 2 * sy));
               }
               return cells;
           }

           private static Vector2Int? GetLastFreeIntermediate(Vector2Int from, Vector2Int lMove, OccupancyMap map)
           {
               var cells = GetIntermediateCells(from, lMove);
               Vector2Int? last = null;
               foreach (var c in cells)
                   if (!map.IsOccupied(c)) last = c;
               return last;
           }

           private static Vector2Int GetStepToward(Vector2Int from, Vector2Int target)
           {
               var delta = target - from;
               return new Vector2Int(System.Math.Sign(delta.x), System.Math.Sign(delta.y));
           }

           private static int Manhattan(Vector2Int a, Vector2Int b)
               => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
       }
   }
   ```
   > **Note on TryMove:** The Knight's L-offset (e.g. `(2,1)`) is passed as `dir` to `GridMovementSystem.TryMove` — the system updates `CurrentCell` by adding dir. This is a single jump, skipping intermediate occupancy checks (per task spec: "ignores occupied intermediate cells").

5. **Create `RookMoveStrategy.cs`** (diagonal, up to 5 cells):
   - 4 diagonal directions: `(1,1), (-1,1), (1,-1), (-1,-1)`
   - For each direction, walk up to 5 cells; stop at first occupied (or player — combat at that cell)
   - Pick direction that brings enemy closest to player's position; return single-step dir in that direction
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class RookMoveStrategy : IMoveStrategy
       {
           private static readonly Vector2Int[] Diagonals = {
               new Vector2Int(1, 1), new Vector2Int(-1, 1),
               new Vector2Int(1, -1), new Vector2Int(-1, -1),
           };

           public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
               => LinearStrategy.GetBestDirection(enemy, ctx, Diagonals, maxSteps: 5);
       }
   }
   ```
   `LinearStrategy` is a shared static helper (defined inside `TowerMoveStrategy.cs` file or a separate `LinearStrategy.cs`):
   ```csharp
   // Shared helper for Rook and Tower
   internal static class LinearStrategy
   {
       internal static Vector2Int? GetBestDirection(
           EnemyView enemy, GameContext ctx, Vector2Int[] directions, int maxSteps)
       {
           var playerCell = ctx.Player?.CurrentCell ?? Vector2Int.zero;
           Vector2Int? bestDir = null;
           int bestDist = int.MaxValue;

           foreach (var dir in directions)
           {
               // Walk up to maxSteps in this direction
               Vector2Int reached = enemy.CurrentCell;
               for (int step = 1; step <= maxSteps; step++)
               {
                   var next = enemy.CurrentCell + dir * step;
                   var occupant = ctx.OccupancyMap.GetEntity(next);
                   if (occupant != null && !(occupant is PlayerView)) break; // blocked by non-player
                   reached = next;
                   if (occupant is PlayerView) break; // stop at player
               }
               if (reached == enemy.CurrentCell) continue; // no movement possible this way

               int dist = Mathf.Abs(reached.x - playerCell.x) + Mathf.Abs(reached.y - playerCell.y);
               if (dist < bestDist) { bestDist = dist; bestDir = dir; }
           }
           return bestDir;
       }
   }
   ```

6. **Create `TowerMoveStrategy.cs`** (straight, up to 5 cells):
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class TowerMoveStrategy : IMoveStrategy
       {
           private static readonly Vector2Int[] Cardinals = {
               new Vector2Int(1, 0), new Vector2Int(-1, 0),
               new Vector2Int(0, 1), new Vector2Int(0, -1),
           };

           public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
               => LinearStrategy.GetBestDirection(enemy, ctx, Cardinals, maxSteps: 5);
       }
   }
   ```
   Place `LinearStrategy` in the same file as `TowerMoveStrategy` or extract to `LinearStrategy.cs`. Keep in `Movement/` folder.

7. **Create `QueenMoveStrategy.cs`** (all 8 directions, up to 6 cells):
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class QueenMoveStrategy : IMoveStrategy
       {
           private static readonly Vector2Int[] AllDirs = {
               new Vector2Int(1, 0), new Vector2Int(-1, 0),
               new Vector2Int(0, 1), new Vector2Int(0, -1),
               new Vector2Int(1, 1), new Vector2Int(-1, 1),
               new Vector2Int(1, -1), new Vector2Int(-1, -1),
           };

           public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
               => LinearStrategy.GetBestDirection(enemy, ctx, AllDirs, maxSteps: 6);
       }
   }
   ```

## Implementation
**Status:** DONE
**Summary:** Created `EnemyMoveState` struct, `IMoveStrategy` interface, `LinearStrategy` shared helper, and all 5 strategy classes in `Core/Systems/Movement/`. Knight handles rank-based crush/fallback; Rook/Tower/Queen delegate to `LinearStrategy`.
