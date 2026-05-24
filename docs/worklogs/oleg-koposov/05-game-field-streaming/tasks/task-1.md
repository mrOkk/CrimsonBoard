# Task 1: Extend configs and ObjectPool

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Configs/BoardConfig.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs`
- Modify: `CB-client/Assets/Scripts/Core/Pools/ObjectPool.cs`

**Commit message:** 05 Extend BoardConfig, PrefabsConfig and ObjectPool for chunk streaming

### Steps

1. В `BoardConfig.cs` добавить два поля после `tileSize`:
   ```csharp
   public int chunkSize = 16;    // tiles per chunk side
   public int windowRadius = 1;  // active window = (2*windowRadius+1)^2 chunks
   ```

2. В `PrefabsConfig.cs` добавить поле `ChunkView chunkPrefab` после `tilePrefab`:
   ```csharp
   public ChunkView chunkPrefab;
   ```
   (По аналогии с `EnemyView enemyPrefab`, `WeaponView weaponPrefab` — каждый пул-префаб типизирован своим View-компонентом.)

3. В `ObjectPool.cs` добавить опциональный `System.Action<T> _onCreate` callback:
   - Добавить поле `private readonly System.Action<T> _onCreate;`
   - Расширить конструктор до `ObjectPool(T prefab, int prewarmCount, System.Action<T> onCreate = null)`, присваивать `_onCreate = onCreate`
   - В методе `Create()` после `Object.Instantiate` вызывать `_onCreate?.Invoke(instance)` перед `SetActive(false)`

## Implementation
<!-- Filled in Phase 3 -->
