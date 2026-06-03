# Task 2: Per-weapon pools

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Pools/GamePools.cs`
- Modify: `CB-client/Assets/Scripts/Core/Pools/PoolConstants.cs`

**Commit message:** 15 Per-weapon pools: Dictionary<int, ObjectPool<WeaponView>> keyed by weapon id

### Steps

1. **PoolConstants.cs** — заменить `Weapons = 10` на `WeaponsPerType = 5` (меньший размер, т.к. отдельный пул на каждое оружие).

2. **GamePools.cs** — заменить `public ObjectPool<WeaponView> Weapons { get; }` на:
   ```csharp
   public IReadOnlyDictionary<int, ObjectPool<WeaponView>> Weapons { get; }
   ```
   В конструкторе убрать `Weapons = new ObjectPool<WeaponView>(prefabs.weaponPrefab, ...)` и добавить построение словаря:
   ```csharp
   var weapons = new Dictionary<int, ObjectPool<WeaponView>>();
   foreach (var entry in prefabs.weaponPrefabs)
   {
       int id = entry.weaponId;
       weapons[id] = new ObjectPool<WeaponView>(
           entry.prefab,
           PoolConstants.WeaponsPerType,
           w => w.SetWeaponId(id)
       );
   }
   Weapons = weapons;
   ```
   Добавить `using System.Collections.Generic;` в начало файла.

3. Добавить вспомогательный метод `GetWeaponPool`:
   ```csharp
   public ObjectPool<WeaponView> GetWeaponPool(int weaponId)
   {
       if (Weapons.TryGetValue(weaponId, out var pool)) return pool;
       UnityEngine.Debug.LogWarning($"[GamePools] No pool for weaponId={weaponId}");
       return null;
   }
   ```
   Прецедент: аналогичный defensive null-return паттерн используется в `EnemySpawnSystem.SpawnEnemyAt`.

## Implementation
<!-- Filled in Phase 3 -->
