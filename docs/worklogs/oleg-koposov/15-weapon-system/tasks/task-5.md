# Task 5: WeaponPickupSystem

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/WeaponPickupSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** 15 WeaponPickupSystem: track dropped weapons, handle pickup trigger, auto-switch

### Steps

1. Создать `WeaponPickupSystem.cs` в `Core/Systems/`:
   ```csharp
   using System.Collections.Generic;
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class WeaponPickupSystem : IGameSystem
       {
           private readonly GameContext _context;
           private readonly List<WeaponView> _dropped = new();

           public WeaponPickupSystem(GameContext context)
           {
               _context = context;
           }

           public void Initialize() { }
           public void Tick(float deltaTime) { }

           public void Dispose()
           {
               foreach (var w in _dropped)
                   w.TriggerEntered = null;
               _dropped.Clear();
           }

           public void RegisterDropped(WeaponView weapon)
           {
               _dropped.Add(weapon);
               weapon.TriggerEntered += OnWeaponTrigger;
           }

           private void OnWeaponTrigger(WeaponView weapon, Collider other)
           {
               if (other.gameObject != _context.Player.gameObject) return;

               var inventory = _context.Inventory;
               var weaponCfg = System.Array.Find(_context.Config.weapons, w => w.id == weapon.WeaponId);
               if (weaponCfg == null) return;

               bool isNew = inventory.TryAddWeapon(weapon.WeaponId);
               int ammo = weaponCfg.infiniteAmmo ? int.MaxValue : weaponCfg.ammoOnPickup;
               inventory.AddAmmo(weapon.WeaponId, ammo);

               if (isNew)
                   inventory.SwitchTo(weapon.WeaponId);

               weapon.TriggerEntered -= OnWeaponTrigger;
               _dropped.Remove(weapon);
               weapon.SetEquippedMode();
               _context.Pools.GetWeaponPool(weapon.WeaponId)?.Return(weapon);
           }
       }
   }
   ```

2. **GameplayState.cs** — добавить поле `private WeaponPickupSystem _weaponPickupSystem;`, создать и зарегистрировать в конструкторе после инициализации `_healthSystem`:
   ```csharp
   _weaponPickupSystem = new WeaponPickupSystem(context);
   _systemRunner.RegisterSystem(_weaponPickupSystem);
   ```
   Добавить публичное свойство `public WeaponPickupSystem WeaponPickupSystem => _weaponPickupSystem;` (аналогично `HealthSystem`).
   Прецедент: регистрация систем — `GameplayState.cs` конструктор.

3. В `HealthSystem.OnEnemyDeath` нужно иметь доступ к `WeaponPickupSystem` для вызова `RegisterDropped`. Для этого HealthSystem получит callback:
   ```csharp
   public System.Action<WeaponView> WeaponDropped;
   ```
   Это событие будет подписано в GameplayState (см. Task 6).

## Implementation
<!-- Filled in Phase 3 -->
