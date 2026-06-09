# Task 2: Migrate GameFieldSystem out of GameContext

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/States/TapToStartState.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** 17 migrate GameFieldSystem to GameBoard

### Steps

1. В `TapToStartState.cs`, сохранить `GameFieldSystem` в локальное поле:
   ```csharp
   private GameFieldSystem _gameFieldSystem;

   // In Enter():
   _gameFieldSystem = new GameFieldSystem(_context);
   _gameFieldSystem.Initialize();
   _context.Board = new GameBoard(_gameFieldSystem);

   new PlayerSpawnSystem(_context).Initialize();
   ```

2. В `TapToStartState.Exit()` сохранить `_gameFieldSystem` или передать дальше (он уже доступен через `_context.Board`).

3. В `GameplayState` конструкторе, заменить `context.GameFieldSystem` на `_context.Board.FieldSystem`:
   ```csharp
   _systemRunner.RegisterSystem(_context.Board.FieldSystem);
   ```

4. Удалить `public GameFieldSystem GameFieldSystem { get; set; }` из `GameContext.cs`.

5. Проверить что нет других ссылок на `context.GameFieldSystem` (grep).

## Implementation
**Status:** DONE
**Summary:** Migrated GameFieldSystem to GameBoard. TapToStartState now creates GameBoard, GameplayState accesses it via context.Board.FieldSystem.
