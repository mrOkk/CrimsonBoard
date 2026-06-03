# Task 6: HealthSystem weapon drop

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Systems/HealthSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** 15 HealthSystem: weighted random weapon drop on enemy death

### Steps

1. **HealthSystem.cs** — добавить `public System.Action<WeaponView> WeaponDropped;` рядом с `EnemyDeathCallback`.

2. В методе `OnEnemyDeath`, после вызова `EnemyDeathCallback`, добавить логику дропа:
   ```csharp
   TryDropWeapon(enemyCell);
   ```

3. Добавить приватный метод `TryDropWeapon`:
   ```csharp
   private void TryDropWeapon(Vector2Int enemyCell)
   {
       var weapons = _context.Config.weapons;
       // Collect weapons with dropChance > 0
       float totalWeight = 0f;
       foreach (var w in weapons)
           if (w.dropChance > 0f) totalWeight += w.dropChance;
       if (totalWeight <= 0f) return;

       float roll = (float)(_context.SharedRandom.NextDouble() * totalWeight);
       float acc = 0f;
       WeaponConfig chosen = null;
       foreach (var w in weapons)
       {
           if (w.dropChance <= 0f) continue;
           acc += w.dropChance;
           if (roll < acc) { chosen = w; break; }
       }
       if (chosen == null) return;

       var pool = _context.Pools.GetWeaponPool(chosen.id);
       if (pool == null) return;

       var weaponView = pool.Get();
       var worldPos = ChunkCoordConverter.TileToWorld(enemyCell, _context.Config.board);
       weaponView.SetDroppedMode(worldPos);
       WeaponDropped?.Invoke(weaponView);
   }
   ```
   Прецедент: аналогичный weighted random — `EnemySpawnSystem.PickEnemyId`.
   Прецедент: `ChunkCoordConverter.TileToWorld` — `EnemySpawnSystem.SpawnEnemyAt`.

4. **GameplayState.cs** — в конструкторе, после создания `_weaponPickupSystem`, связать событие:
   ```csharp
   _healthSystem.WeaponDropped += _weaponPickupSystem.RegisterDropped;
   ```

## Implementation
**Status:** DONE
**Summary:** `HealthSystem` получил `WeaponDropped` event и `TryDropWeapon` — weighted random выбор среди оружий с `dropChance > 0`, спавн в dropped-режиме. `GameplayState` подписал `_weaponPickupSystem.RegisterDropped` на этот event.
