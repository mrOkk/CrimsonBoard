# Task 3: GameFieldSystem and GameplayState wiring

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/GameField/GameFieldSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** 05 Add GameFieldSystem and wire into GameplayState

### Steps

1. Создать `GameFieldSystem.cs` в `Core/GameField/`. Класс `GameFieldSystem : IGameSystem`:

   **Поля:**
   ```csharp
   private readonly GameContext _context;
   private ObjectPool<ChunkView> _chunkPool;
   private readonly Dictionary<Vector2Int, ChunkView> _activeChunks = new Dictionary<Vector2Int, ChunkView>();
   private Vector2Int _currentCenter;
   ```

   **Конструктор** `GameFieldSystem(GameContext context)` — сохраняет `_context`.

   **`Initialize()`:**
   - Вычислить размер pre-warm: `int diameter = 2 * _context.Config.board.windowRadius + 1; int prewarm = diameter * diameter + 4;`
   - Создать пул с onCreate callback:
     ```csharp
     _chunkPool = new ObjectPool<ChunkView>(
         _context.Config.prefabs.chunkPrefab,
         prewarm,
         chunk => chunk.Initialize(_context.Config.board.chunkSize, _context.Config.prefabs.tilePrefab)
     );
     ```
   - Загрузить стартовое окно: `_currentCenter = Vector2Int.zero; LoadWindow(Vector2Int.zero);`

   **`Tick(float deltaTime)`** — пустой (обновление окна вызывается извне через `OnPlayerChunkChanged`).

   **`Dispose()`** — вернуть все активные чанки в пул:
   ```csharp
   foreach (var kvp in _activeChunks) { kvp.Value.Clear(); _chunkPool.Return(kvp.Value); }
   _activeChunks.Clear();
   ```

   **`public void OnPlayerChunkChanged(Vector2Int newCenter)`:**
   - `if (newCenter == _currentCenter) return;`
   - `_currentCenter = newCenter;`
   - `UpdateWindow(newCenter);`

   **`private void LoadWindow(Vector2Int center)`** — для каждого `(dx, dy)` в диапазоне `[-windowRadius, +windowRadius]` вызвать `LoadChunk(center + new Vector2Int(dx, dy))`.

   **`private void UpdateWindow(Vector2Int newCenter)`:**
   - Собрать координаты нового окна в `HashSet<Vector2Int>`
   - Найти активные чанки вне нового окна → `Clear()` + `_chunkPool.Return()` + убрать из словаря
   - Загрузить новые координаты окна, которых нет в `_activeChunks`: вызвать `LoadChunk(coord)`

   **`private void LoadChunk(Vector2Int coord)`:**
   - `var chunk = _chunkPool.Get();`
   - `chunk.Setup(coord, _context.Config.board);`
   - `_activeChunks[coord] = chunk;`

2. В `GameplayState.cs` создать и зарегистрировать систему. В конструкторе после `_systemRunner = new GameplaySystemRunner()`:
   ```csharp
   _systemRunner.RegisterSystem(new GameFieldSystem(context));
   ```
   Добавить `using`-директиву если нужна (namespace тот же — `CrimsonBoard`).

## Implementation
**Status:** DONE
**Summary:** Создан `GameFieldSystem : IGameSystem` с внутренним пулом чанков, `LoadWindow`/`UpdateWindow` логикой и точкой интеграции `OnPlayerChunkChanged`. `GameplayState` регистрирует систему в конструкторе.
