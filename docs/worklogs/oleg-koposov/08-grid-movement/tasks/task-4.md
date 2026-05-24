# Task 4: Wiring — PlayerSpawnSystem, HealthSystem knockback, GameplayState

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Systems/PlayerSpawnSystem.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/HealthSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** `08 Wire GridMovementSystem and PlayerMovementSystem into GameplayState`

### Steps

1. В `PlayerSpawnSystem.cs` после присвоения `_context.Player` (оба пути — спавн и респавн) добавить:
   ```csharp
   var spawnCell = ChunkCoordConverter.WorldToTile(_context.Player.transform.position, _context.Config.board);
   _context.Player.CurrentCell = spawnCell;
   _context.OccupancyMap.Register(spawnCell, _context.Player);
   ```
   Для ре-спавна сначала снять предыдущую регистрацию:
   ```csharp
   _context.OccupancyMap.Unregister(_context.Player.CurrentCell);
   ```
   перед `transform.position = spawnPos`.

2. В `HealthSystem.cs` заменить TODO-комментарий после `OccupancyMap.Unregister(playerCell)` на реальное перемещение:
   ```csharp
   _context.Player.CurrentCell = targetCell.Value;
   _context.Player.transform.position = ChunkCoordConverter.TileToWorld(targetCell.Value, _context.Config.board);
   _context.OccupancyMap.Register(targetCell.Value, _context.Player);
   ```

3. В `GameplayState.cs`:
   - Добавить поле `private GridMovementSystem _gridMovementSystem;`
   - В конструкторе создать и зарегистрировать системы в правильном порядке (GridMovementSystem должен быть создан до PlayerMovementSystem и HealthSystem, потому что оба на него ссылаются):
     ```csharp
     _gridMovementSystem = new GridMovementSystem(context);
     _systemRunner.RegisterSystem(_gridMovementSystem);
     _systemRunner.RegisterSystem(new PlayerSpawnSystem(context));
     _healthSystem = new HealthSystem(context, fsm);
     _gridMovementSystem.HealthSystem = _healthSystem;   // inject after both created
     _systemRunner.RegisterSystem(_healthSystem);
     _systemRunner.RegisterSystem(new PlayerMovementSystem(context, _gridMovementSystem));
     ```
   - Убрать дублирующие старые строки регистрации `GameFieldSystem`, `PlayerSpawnSystem`, `HealthSystem` (они теперь переупорядочены; `GameFieldSystem` остаётся первым).
   - Добавить публичное свойство `public GridMovementSystem GridMovementSystem => _gridMovementSystem;`.

## Implementation
<!-- Filled in Phase 3 -->
