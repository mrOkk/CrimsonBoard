# Task 2: ChunkView and ChunkCoordConverter

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Entities/ChunkView.cs`
- Create: `CB-client/Assets/Scripts/Core/GameField/ChunkCoordConverter.cs`

**Commit message:** 05 Add ChunkView and ChunkCoordConverter

### Steps

1. Создать папку `CB-client/Assets/Scripts/Core/GameField/`.

2. Создать `ChunkView.cs` в `Entities/`. Класс `ChunkView : MonoBehaviour`:
   - Приватные поля: `BoardTileView[] _tiles`, `bool _initialized`
   - Метод `Initialize(int chunkSize, BoardTileView tilePrefab)`:
     - Guard: `if (_initialized) return;`
     - Создать `chunkSize * chunkSize` экземпляров `tilePrefab` как дочерних к `transform` через `Object.Instantiate(tilePrefab, transform)`
     - Записать в `_tiles`, установить `_initialized = true`, все тайлы `SetActive(false)`
   - Метод `Setup(Vector2Int coord, BoardConfig config)`:
     - Вычислить мировую позицию чанка: `ChunkCoordConverter.ChunkToWorld(coord, config)`
     - Установить `transform.position`
     - Расставить тайлы по сетке: для каждого тайла `(row, col)` вычислить локальную позицию `new Vector3(col * config.tileSize.x, 0, row * config.tileSize.y)`, задать `tile.transform.localPosition`, назначить меш `(row + col) % 2 == 0 ? config.darkTile : config.lightTile` через `tile.MeshFilter.sharedMesh`, вызвать `tile.gameObject.SetActive(true)`
   - Метод `Clear()`:
     - Деактивировать все тайлы: `foreach (var t in _tiles) t.gameObject.SetActive(false);`

3. Создать `ChunkCoordConverter.cs` в `Core/GameField/`. Статический класс:
   - `public static Vector2Int WorldToChunk(Vector3 worldPos, BoardConfig config)`:
     - `float chunkWorldSize_x = config.chunkSize * config.tileSize.x;`
     - `float chunkWorldSize_z = config.chunkSize * config.tileSize.y;`
     - `return new Vector2Int(Mathf.FloorToInt(worldPos.x / chunkWorldSize_x), Mathf.FloorToInt(worldPos.z / chunkWorldSize_z));`
   - `public static Vector3 ChunkToWorld(Vector2Int coord, BoardConfig config)`:
     - `float chunkWorldSize_x = config.chunkSize * config.tileSize.x;`
     - `float chunkWorldSize_z = config.chunkSize * config.tileSize.y;`
     - `return new Vector3(coord.x * chunkWorldSize_x, 0f, coord.y * chunkWorldSize_z);`

## Implementation
<!-- Filled in Phase 3 -->
