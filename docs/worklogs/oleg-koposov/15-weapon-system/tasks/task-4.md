# Task 4: PlayerInventory + init

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Inventory/PlayerInventory.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/PlayerSpawnSystem.cs`

**Commit message:** 15 PlayerInventory: plain C# class, init from config in PlayerSpawnSystem

### Steps

1. Создать папку `Core/Inventory/` и файл `PlayerInventory.cs`:
   ```csharp
   using System.Collections.Generic;

   namespace CrimsonBoard
   {
       public class PlayerInventory
       {
           private readonly List<int> _weaponIds = new();
           private readonly Dictionary<int, int> _ammo = new();

           public IReadOnlyList<int> WeaponIds => _weaponIds;
           public int? ActiveWeaponId { get; private set; }

           public bool HasWeapon(int weaponId) => _weaponIds.Contains(weaponId);

           /// <returns>true if the weapon was newly added (not previously owned)</returns>
           public bool TryAddWeapon(int weaponId)
           {
               if (HasWeapon(weaponId)) return false;
               _weaponIds.Add(weaponId);
               if (ActiveWeaponId == null) ActiveWeaponId = weaponId;
               return true;
           }

           public void AddAmmo(int weaponId, int amount)
           {
               _ammo.TryGetValue(weaponId, out int current);
               _ammo[weaponId] = current + amount;
           }

           public int GetAmmo(int weaponId) =>
               _ammo.TryGetValue(weaponId, out int v) ? v : 0;

           public void SwitchTo(int weaponId)
           {
               if (HasWeapon(weaponId)) ActiveWeaponId = weaponId;
           }
       }
   }
   ```
   Прецедент: аналогичный plain-class подход используется для `GameStats` и `InputState` в `GameContext`.

2. **GameContext.cs** — добавить свойство:
   ```csharp
   public PlayerInventory Inventory { get; set; }
   ```

3. **PlayerSpawnSystem.cs** — в методе `Initialize()`, после того как `_context.Player` назначен, инициализировать инвентарь:
   ```csharp
   var inventory = new PlayerInventory();
   foreach (var entry in _context.Config.player.startingInventory)
   {
       var weaponCfg = System.Array.Find(_context.Config.weapons, w => w.id == entry.weaponId);
       if (weaponCfg == null) continue;
       inventory.TryAddWeapon(entry.weaponId);
       int ammo = weaponCfg.infiniteAmmo ? int.MaxValue : entry.ammoCount;
       inventory.AddAmmo(entry.weaponId, ammo);
   }
   _context.Inventory = inventory;
   ```
   Прецедент: паттерн `Array.Find` по id — в `EnemySpawnSystem.SpawnEnemyAt`.

## Implementation
<!-- Filled in Phase 3 -->
