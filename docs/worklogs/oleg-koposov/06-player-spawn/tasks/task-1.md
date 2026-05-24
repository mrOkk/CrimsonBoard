# Task 1: PlayerSpawnSystem, GameContext and GameplayState

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/PlayerSpawnSystem.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** 06 Add PlayerSpawnSystem and wire into GameContext and GameplayState

### Steps

1. Создать папку `CB-client/Assets/Scripts/Core/Systems/`.

2. Создать `PlayerSpawnSystem.cs` в `Core/Systems/`. Класс `PlayerSpawnSystem : IGameSystem`:

   **Конструктор** `PlayerSpawnSystem(GameContext context)` — сохраняет `_context`.

   **Вспомогательный метод `private Vector3 GetSpawnPosition()`:**
   ```csharp
   var board = _context.Config.board;
   return new Vector3(
       board.chunkSize * board.tileSize.x / 2f,
       0f,
       board.chunkSize * board.tileSize.y / 2f
   );
   ```

   **`Initialize()`:**
   ```csharp
   var spawnPos = GetSpawnPosition();
   if (_context.Player == null)
   {
       var player = Object.Instantiate(_context.Config.prefabs.playerPrefab, spawnPos, Quaternion.identity);
       player.MeshFilter.sharedMesh = _context.Config.player.mesh;
       _context.Player = player;
       Debug.Log("[PlayerSpawnSystem] Player spawned.");
   }
   else
   {
       _context.Player.transform.position = spawnPos;
       Debug.Log("[PlayerSpawnSystem] Player respawned (repositioned).");
   }
   ```

   **`Tick(float deltaTime)`** — пустой.

   **`Dispose()`** — пустой (игрок не уничтожается при выходе из GameplayState, чтобы ре-спавн мог его переместить).

3. В `GameContext.cs` добавить свойство после `Pools`:
   ```csharp
   public PlayerView Player { get; set; }
   ```

4. В `GameplayState.cs` зарегистрировать систему после `GameFieldSystem` в конструкторе:
   ```csharp
   _systemRunner.RegisterSystem(new PlayerSpawnSystem(context));
   ```

## Implementation
**Status:** DONE
**Summary:** Создан `PlayerSpawnSystem`: первый спавн через `Instantiate` с назначением меша, ре-спавн через reposition. `GameContext.Player` добавлен, `GameplayState` регистрирует систему после `GameFieldSystem`.
