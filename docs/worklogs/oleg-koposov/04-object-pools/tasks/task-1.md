# Task 1: Core pool infrastructure

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Pools/ObjectPool.cs`
- Create: `CB-client/Assets/Scripts/Core/Pools/PoolConstants.cs`
- Create: `CB-client/Assets/Scripts/Core/Pools/GamePools.cs`
- Create: `CB-client/Assets/Scripts/Entities/ProjectileView.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs`

**Commit message:** 04 Add core pool infrastructure (ObjectPool, PoolConstants, GamePools, ProjectileView)

### Steps

1. Создать `PoolConstants.cs` в `Core/Pools/`:
   ```csharp
   namespace CrimsonBoard
   {
       public static class PoolConstants
       {
           public const int Enemies = 20;
           public const int Weapons = 10;
           public const int Projectiles = 50;
           public const int PowerUps = 10;
       }
   }
   ```

2. Создать `ObjectPool.cs` в `Core/Pools/`. Поля: `_prefab`, `_container` (Transform скрытого GameObject), `_available` (Queue<T>). Методы:
   - Конструктор `(T prefab, int prewarmCount)` — создаёт контейнер `new GameObject($"[Pool] {typeof(T).Name}").transform`, вызывает `Prewarm(prewarmCount)`.
   - `private T Create()` — `Object.Instantiate(_prefab, _container)`, `SetActive(false)`, возвращает инстанс.
   - `private void Prewarm(int count)` — N раз вызывает `Create()` и кладёт в `_available`.
   - `public T Get()` — берёт из `_available` (или создаёт новый), вызывает `SetActive(true)`, возвращает.
   - `public void Return(T obj)` — `SetActive(false)`, `SetParent(_container)`, кладёт в `_available`.

3. Создать `ProjectileView.cs` в `Entities/` — пустой `MonoBehaviour` (stub, аналог `EnemyView`):
   ```csharp
   using UnityEngine;
   namespace CrimsonBoard
   {
       public class ProjectileView : MonoBehaviour { }
   }
   ```

4. Обновить `PrefabsConfig.cs` — заменить `public GameObject projectilePrefab;` на `public ProjectileView projectilePrefab;` (единообразно с остальными полями, см. `EnemyView enemyPrefab` как образец).

5. Создать `GamePools.cs` в `Core/Pools/`. Конструктор принимает `PrefabsConfig prefabs`. Четыре публичных свойства:
   - `public ObjectPool<EnemyView> Enemies { get; }`
   - `public ObjectPool<WeaponView> Weapons { get; }`
   - `public ObjectPool<ProjectileView> Projectiles { get; }`
   - `public ObjectPool<PowerUpView> PowerUps { get; }`
   
   Конструктор инициализирует каждый пул с соответствующим префабом и константой из `PoolConstants`.

## Implementation
<!-- Filled in Phase 3 -->
