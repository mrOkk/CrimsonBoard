# Task 2: GridMovementSystem — MoveResult, TryMove, OccupancyMap, HealthSystem

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/MoveResult.cs`
- Create: `CB-client/Assets/Scripts/Core/Systems/GridMovementSystem.cs`

**Commit message:** `08 Add MoveResult and GridMovementSystem`

### Steps

1. Создать `Core/Systems/MoveResult.cs`:
   ```csharp
   namespace CrimsonBoard
   {
       public enum MoveResult
       {
           Moved,    // entity moved successfully
           Blocked,  // target cell is occupied by a non-combat entity or out of interest
           Combat    // enemy entered player cell (or vice versa) — HealthSystem was notified
       }
   }
   ```

2. Создать `Core/Systems/GridMovementSystem.cs`. Система получает `GameContext` в конструкторе и `HealthSystem` устанавливается позже через свойство (чтобы избежать циклической зависимости в конструкторе GameplayState).

   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class GridMovementSystem : IGameSystem
       {
           private readonly GameContext _context;
           public HealthSystem HealthSystem { get; set; }

           public GridMovementSystem(GameContext context)
           {
               _context = context;
           }

           public void Initialize() { }
           public void Tick(float deltaTime) { }
           public void Dispose() { }

           /// <summary>
           /// Attempts to move <paramref name="entity"/> one step in <paramref name="dir"/>.
           /// Updates OccupancyMap, transform, and CurrentCell on success.
           /// Triggers HealthSystem when an enemy steps into the player's cell.
           /// </summary>
           public MoveResult TryMove(EntityView entity, Vector2Int dir)
           {
               var targetCell = entity.CurrentCell + dir;
               var occupant = _context.OccupancyMap.GetEntity(targetCell);

               if (occupant != null)
               {
                   // Combat: enemy steps into player cell
                   if (entity is EnemyView enemy && occupant is PlayerView)
                   {
                       HealthSystem?.ApplyDamageToPlayer(enemy, dir, occupant.CurrentCell);
                       return MoveResult.Combat;
                   }
                   return MoveResult.Blocked;
               }

               // Move
               _context.OccupancyMap.Unregister(entity.CurrentCell);
               entity.CurrentCell = targetCell;
               entity.transform.position = ChunkCoordConverter.TileToWorld(targetCell, _context.Config.board);
               _context.OccupancyMap.Register(targetCell, entity);
               return MoveResult.Moved;
           }
       }
   }
   ```

## Implementation
<!-- Filled in Phase 3 -->
