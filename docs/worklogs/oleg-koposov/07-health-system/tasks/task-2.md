# Task 2: OccupancyMap, GameContext, KnockbackResolver

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/OccupancyMap.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/KnockbackResolver.cs`

**Commit message:** `07 Add OccupancyMap to GameContext and KnockbackResolver`

### Steps

1. Создать `Core/OccupancyMap.cs`:
   ```csharp
   using System.Collections.Generic;
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class OccupancyMap
       {
           private readonly Dictionary<Vector2Int, EntityView> _cells = new Dictionary<Vector2Int, EntityView>();

           public void Register(Vector2Int cell, EntityView entity) => _cells[cell] = entity;

           public void Unregister(Vector2Int cell) => _cells.Remove(cell);

           public bool IsOccupied(Vector2Int cell) => _cells.ContainsKey(cell);

           public EntityView GetEntity(Vector2Int cell)
           {
               _cells.TryGetValue(cell, out var entity);
               return entity;
           }
       }
   }
   ```

2. В `Core/GameContext.cs` добавить в конструктор инициализацию:
   ```csharp
   public OccupancyMap OccupancyMap { get; }
   ```
   и в конструктор `GameContext(GameConfig config)`:
   ```csharp
   OccupancyMap = new OccupancyMap();
   ```

3. Создать `Core/Systems/KnockbackResolver.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public static class KnockbackResolver
       {
           // Fixed priority order for fallback cells (deterministic)
           private static readonly Vector2Int[] FallbackDirs = new[]
           {
               new Vector2Int(0, 1),   // North
               new Vector2Int(0, -1),  // South
               new Vector2Int(1, 0),   // East
               new Vector2Int(-1, 0),  // West
           };

           /// <summary>
           /// Returns the target cell to push the player to, or null if all adjacent cells are occupied.
           /// </summary>
           /// <param name="playerPos">Current player cell.</param>
           /// <param name="enemyDir">Direction the enemy was moving when it hit the player.</param>
           /// <param name="map">Current occupancy map.</param>
           public static Vector2Int? Resolve(Vector2Int playerPos, Vector2Int enemyDir, OccupancyMap map)
           {
               // Primary: opposite of enemy movement direction
               var primary = playerPos - enemyDir;
               if (!map.IsOccupied(primary))
                   return primary;

               // Fallback: fixed priority order, skip primary and current position
               foreach (var dir in FallbackDirs)
               {
                   var candidate = playerPos + dir;
                   if (candidate == primary) continue;
                   if (!map.IsOccupied(candidate))
                       return candidate;
               }

               return null; // All occupied — no knockback, only damage
           }
       }
   }
   ```

## Implementation
**Status:** DONE
**Summary:** Создан `OccupancyMap` (Dictionary<Vector2Int, EntityView> с Register/Unregister/IsOccupied/GetEntity). `GameContext` получил `OccupancyMap` property, инициализируемый в конструкторе. Создан `KnockbackResolver` (static class) с детерминированным fallback (primary: opposite, затем N/S/E/W).
