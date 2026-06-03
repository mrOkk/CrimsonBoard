# Task 1: Config types refactor

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Configs/WeaponConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/WeaponPrefabEntry.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/InventoryEntry.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/PlayerConfig.cs`

**Commit message:** 15 Config types refactor: per-weapon prefab entries, inventory entry, remove mesh from WeaponConfig

### Steps

1. **WeaponConfig.cs** — удалить поле `public Mesh mesh;`, добавить три новых поля:
   ```csharp
   public bool infiniteAmmo;
   public int ammoOnPickup;
   [Range(0f, 1f)] public float dropChance;
   ```
   Прецедент: аналогично другим полям в `EnemyConfig.cs`.

2. **WeaponPrefabEntry.cs** — создать файл в `Core/Configs/`:
   ```csharp
   using UnityEngine;
   namespace CrimsonBoard
   {
       [System.Serializable]
       public class WeaponPrefabEntry
       {
           public int weaponId;
           public WeaponView prefab;
       }
   }
   ```

3. **InventoryEntry.cs** — создать файл в `Core/Configs/`:
   ```csharp
   namespace CrimsonBoard
   {
       [System.Serializable]
       public class InventoryEntry
       {
           public int weaponId;
           public int ammoCount;
       }
   }
   ```

4. **PrefabsConfig.cs** — заменить `public WeaponView weaponPrefab;` на:
   ```csharp
   public WeaponPrefabEntry[] weaponPrefabs;
   ```

5. **PlayerConfig.cs** — добавить поле после `inputBufferWindow`:
   ```csharp
   public InventoryEntry[] startingInventory;
   ```

## Implementation
**Status:** DONE
**Summary:** Удалены `mesh` из `WeaponConfig`, добавлены `infiniteAmmo`, `ammoOnPickup`, `dropChance`. Созданы `WeaponPrefabEntry` и `InventoryEntry`. `PrefabsConfig.weaponPrefab` заменён на `weaponPrefabs[]`. `PlayerConfig` получил `startingInventory[]`.
