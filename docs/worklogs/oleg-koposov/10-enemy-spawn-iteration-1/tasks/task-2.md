# Task 2: SharedRandom + GetBorderTiles

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameField/GameFieldSystem.cs`

**Commit message:** `10 Add SharedRandom to GameContext and GetBorderTiles to GameFieldSystem`

### Steps

1. **Modify `GameContext.cs`** — add `SharedRandom` property and initialise it from `SpawnConfig.randomSeed` in the constructor:
   ```csharp
   public System.Random SharedRandom { get; private set; }
   ```
   In the constructor body, after `OccupancyMap = new OccupancyMap();`:
   ```csharp
   SharedRandom = new System.Random(config.spawn.randomSeed);
   ```
   This makes the RNG available to any system that holds a `GameContext` reference, following the existing pattern (all shared state lives in `GameContext`).

2. **Add `GetBorderTiles()` to `GameFieldSystem.cs`** — returns the tile coordinates that form the outer ring of the currently active chunk window. The active window is a square of `(2*windowRadius+1)` chunks centred on `_currentCenter`.

   Border tile range (inclusive) in tile-space:
   - `int r = windowRadius * chunkSize`
   - `minX = (_currentCenter.x - windowRadius) * chunkSize`
   - `maxX = (_currentCenter.x + windowRadius + 1) * chunkSize - 1`
   - Same for Y
   
   Return only tiles on the perimeter: `x == minX || x == maxX || y == minY || y == maxY`.

   ```csharp
   /// <summary>
   /// Returns all tile coordinates on the outer ring of the active chunk window.
   /// </summary>
   public List<Vector2Int> GetBorderTiles()
   {
       var board = _context.Config.board;
       int r = board.windowRadius;
       int cs = board.chunkSize;

       int minX = (_currentCenter.x - r) * cs;
       int maxX = (_currentCenter.x + r + 1) * cs - 1;
       int minY = (_currentCenter.y - r) * cs;
       int maxY = (_currentCenter.y + r + 1) * cs - 1;

       var result = new List<Vector2Int>();
       for (int x = minX; x <= maxX; x++)
       {
           result.Add(new Vector2Int(x, minY));
           result.Add(new Vector2Int(x, maxY));
       }
       for (int y = minY + 1; y <= maxY - 1; y++)
       {
           result.Add(new Vector2Int(minX, y));
           result.Add(new Vector2Int(maxX, y));
       }
       return result;
   }
   ```
   Add `using System.Collections.Generic;` — already present in `GameFieldSystem.cs`.

## Implementation
**Status:** DONE
**Summary:** `SharedRandom` добавлен в `GameContext`, инициализируется из `config.spawn.randomSeed`. В `GameFieldSystem` добавлены `GetBorderTiles()` (instance) и `ComputeBorderTiles()` (static, для тестов).
