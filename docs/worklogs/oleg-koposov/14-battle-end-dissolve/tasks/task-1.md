# Task 1: DissolveService + HealthSystem refactor

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/DissolveService.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/HealthSystem.cs`

**Commit message:** `14 Extract DissolveService, refactor HealthSystem.OnEnemyDeath`

### Steps

1. **Создать `DissolveService.cs`** в `CB-client/Assets/Scripts/Core/Systems/`:
   ```csharp
   using System;
   using System.Collections.Generic;

   namespace CrimsonBoard
   {
       public static class DissolveService
       {
           /// <summary>
           /// Unregisters enemy from the occupancy map, disables its collider,
           /// plays the dissolve animation and returns the enemy to the pool on completion.
           /// </summary>
           public static void DissolveAndReturn(
               EnemyView enemy,
               Vector2Int cell,
               OccupancyMap occupancyMap,
               GamePools pools,
               Action onComplete = null)
           {
               occupancyMap.Unregister(cell);
               enemy.Collider.enabled = false;
               enemy.Dissolve.Play(() =>
               {
                   enemy.Collider.enabled = true;
                   pools.Enemies.Return(enemy);
                   onComplete?.Invoke();
               });
           }

           /// <summary>
           /// Dissolves all enemies in the snapshot list and returns each to the pool.
           /// <paramref name="onAllComplete"/> is invoked once the last dissolve finishes.
           /// Safe to call with an empty list (onAllComplete invoked immediately).
           /// </summary>
           public static void DissolveAllAndReturn(
               IReadOnlyList<EnemyView> enemies,
               OccupancyMap occupancyMap,
               GamePools pools,
               Action onAllComplete = null)
           {
               if (enemies.Count == 0)
               {
                   onAllComplete?.Invoke();
                   return;
               }

               int remaining = enemies.Count;
               // Snapshot cells before any mutations
               var cells = new Vector2Int[enemies.Count];
               for (int i = 0; i < enemies.Count; i++)
                   cells[i] = enemies[i].CurrentCell;

               for (int i = 0; i < enemies.Count; i++)
               {
                   var enemy = enemies[i];
                   var cell = cells[i];
                   DissolveAndReturn(enemy, cell, occupancyMap, pools, () =>
                   {
                       if (--remaining == 0)
                           onAllComplete?.Invoke();
                   });
               }
           }
       }
   }
   ```
   Обратить внимание: `Vector2Int` используется из `UnityEngine` — добавить `using UnityEngine;`.

2. **Изменить `HealthSystem.OnEnemyDeath`** — заменить inline dissolve-логику вызовом `DissolveService.DissolveAndReturn`:
   ```csharp
   public void OnEnemyDeath(EnemyView enemy, Vector2Int enemyCell)
   {
       EnemyDeathCallback?.Invoke(enemy);
       DissolveService.DissolveAndReturn(enemy, enemyCell, _context.OccupancyMap, _context.Pools);
   }
   ```
   Убедиться, что прежние строки `_context.OccupancyMap.Unregister`, `enemy.Collider.enabled = false`, `enemy.Dissolve.Play(...)` удалены.

## Implementation

<!-- Filled in Phase 3 -->
