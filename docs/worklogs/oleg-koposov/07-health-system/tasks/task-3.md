# Task 3: HealthSystem and GameplayState wiring

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/HealthSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** `07 Add HealthSystem and register in GameplayState`

### Steps

1. Создать `Core/Systems/HealthSystem.cs`. Система принимает `GameContext` и `GameStateMachine`. Два публичных метода — `ApplyDamageToPlayer` и `OnEnemyDeath` — вызываются извне (из будущей системы движения врагов):
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class HealthSystem : IGameSystem
       {
           private readonly GameContext _context;
           private readonly GameStateMachine _fsm;

           public HealthSystem(GameContext context, GameStateMachine fsm)
           {
               _context = context;
               _fsm = fsm;
           }

           public void Initialize()
           {
               var config = _context.Config.player;
               _context.Player.Health.Init(config.health);

               // TODO: subscribe to enemy-entered-cell events when enemy movement is implemented
           }

           public void Tick(float deltaTime) { }

           public void Dispose()
           {
               // TODO: unsubscribe from events
           }

           /// <summary>
           /// Call this when an enemy enters the player's cell.
           /// </summary>
           /// <param name="enemy">The attacking enemy.</param>
           /// <param name="enemyDir">Grid direction the enemy was moving.</param>
           /// <param name="playerCell">Current cell of the player.</param>
           public void ApplyDamageToPlayer(EnemyView enemy, Vector2Int enemyDir, Vector2Int playerCell)
           {
               _context.Player.Health.TakeDamage(enemy.Config.damage);

               if (_context.Player.Health.IsDead)
               {
                   _fsm.ChangeState(new GameOverState(_context, _fsm));
                   return;
               }

               var targetCell = KnockbackResolver.Resolve(playerCell, enemyDir, _context.OccupancyMap);
               if (targetCell.HasValue)
               {
                   _context.OccupancyMap.Unregister(playerCell);
                   // TODO: move player transform to world position of targetCell (needs ChunkCoordConverter or tileSize)
                   _context.OccupancyMap.Register(targetCell.Value, _context.Player);
               }
               // else: all cells occupied — only damage applied, player stays
           }

           /// <summary>
           /// Call this when an enemy's HP reaches zero.
           /// </summary>
           public void OnEnemyDeath(EnemyView enemy, Vector2Int enemyCell)
           {
               _context.OccupancyMap.Unregister(enemyCell);
               _context.Pools.Enemies.Return(enemy);
           }
       }
   }
   ```

2. В `GameplayState.cs` добавить поле `private HealthSystem _healthSystem;`, создать его в конструкторе после `PlayerSpawnSystem`:
   ```csharp
   _healthSystem = new HealthSystem(context, fsm);
   _systemRunner.RegisterSystem(_healthSystem);
   ```
   Добавить публичное свойство `public HealthSystem HealthSystem => _healthSystem;` чтобы другие системы могли вызывать `ApplyDamageToPlayer`.

## Implementation
**Status:** DONE
**Summary:** Создан `HealthSystem` (IGameSystem) с публичными методами `ApplyDamageToPlayer` (урон + knockback через KnockbackResolver, переход в GameOverState при смерти) и `OnEnemyDeath` (Unregister + return в пул). `GameplayState` регистрирует `HealthSystem` и экспонирует его через публичное свойство `HealthSystem`.
