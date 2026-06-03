# 15 Weapon System

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `15` |
| `task_file` | `docs/tasks/15-weapon-system.md` |
| `branch` | `feature/15-weapon-system` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-06-03T08:44:20Z` |
| `updated_at` | `2026-06-03T09:15:42Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-06-03T09:15:42Z | 2 → 3 | Plan signed off |
| 2026-06-03T09:08:24Z | 1 → 2 | Discovery signed off |
| 2026-06-03T08:44:20Z | — → 1 | Created by wf-start |

## Discovery

### Chosen approach
Единая реализация: каждое оружие — отдельный префаб с собственным пулом, инвентарь как plain C# класс в GameContext, подбор через trigger-событие на WeaponView, выпадение при смерти врага через weighted random.

### Scope IN
- Удалить `mesh` из `WeaponConfig`, добавить `infiniteAmmo`, `ammoOnPickup`, `dropChance`
- `WeaponPrefabEntry[]` в `PrefabsConfig`, `Dictionary<int, ObjectPool<WeaponView>>` в `GamePools`
- `WeaponView`: добавить `WeaponId`, `_hoverHeight`, `_pickupCollider` (trigger), `OnTriggerEnter → TriggerEntered event`, методы `SetDroppedMode` / `SetEquippedMode`
- `InventoryEntry` + расширение `PlayerConfig` начальным инвентарём
- `PlayerInventory` — plain C# класс в `GameContext`
- `WeaponPickupSystem` — IGameSystem (триггеры, auto-switch)
- `HealthSystem` — weighted random drop при смерти врага (макс. 1 оружие)
- `PlayerSpawnSystem` — инициализация инвентаря из конфига
- `[WeaponId]` атрибут + `WeaponIdDrawer` PropertyDrawer; `GameConfig.EditorInstance` с кешированием

### Scope OUT
- Стрельба, переключение оружия игроком — отдельная задача

### Key constraints
- Существующий ObjectPool<T> переиспользуется без изменений
- WeaponView остаётся dumb view: логика только через события/коллбэки
- `infiniteAmmo` в инвентаре хранится как `int.MaxValue` для совместимости с общей ammo-map

### Files likely touched
**Modified:** `WeaponConfig.cs`, `WeaponView.cs`, `PrefabsConfig.cs`, `GamePools.cs`, `PlayerConfig.cs`, `GameContext.cs`, `PlayerSpawnSystem.cs`, `HealthSystem.cs`, `GameplayState.cs`, `GameConfig.cs`  
**New:** `WeaponPrefabEntry.cs`, `InventoryEntry.cs`, `PlayerInventory.cs`, `WeaponPickupSystem.cs`, `WeaponIdAttribute.cs`, `WeaponIdDrawer.cs` (Editor)

## Tasks

**Goal:** Реализовать базовую систему оружия и инвентаря: каждое оружие — отдельный префаб со своим пулом, игрок имеет инвентарь с патронами, оружие подбирается через trigger-коллайдер, враги роняют оружие при смерти.

**Architecture:** Конфиги описывают оружие без меш-данных (меш в префабе). GamePools хранит `Dictionary<int, ObjectPool<WeaponView>>` — по одному пулу на weapon id. PlayerInventory — plain C# класс в GameContext, инициализируется PlayerSpawnSystem из конфига. WeaponPickupSystem (IGameSystem) подписывается на события WeaponView и выполняет всю логику подбора. HealthSystem при смерти врага делает weighted random roll и спавнит оружие в dropped-режиме.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `Assets/Scripts/Core/Configs/WeaponConfig.cs` | Modify | Удалить mesh, добавить infiniteAmmo/ammoOnPickup/dropChance |
| `Assets/Scripts/Core/Configs/WeaponPrefabEntry.cs` | Create | [Serializable] пара weaponId + WeaponView prefab |
| `Assets/Scripts/Core/Configs/InventoryEntry.cs` | Create | [Serializable] пара weaponId + ammoCount для начального инвентаря |
| `Assets/Scripts/Core/Configs/PrefabsConfig.cs` | Modify | Заменить weaponPrefab на WeaponPrefabEntry[] weaponPrefabs |
| `Assets/Scripts/Core/Configs/PlayerConfig.cs` | Modify | Добавить InventoryEntry[] startingInventory |
| `Assets/Scripts/Core/Pools/GamePools.cs` | Modify | Weapons → Dictionary<int, ObjectPool<WeaponView>> |
| `Assets/Scripts/Core/Pools/PoolConstants.cs` | Modify | Добавить WeaponsPerType для per-weapon прогрева |
| `Assets/Scripts/Entities/WeaponView.cs` | Modify | Добавить WeaponId, _hoverHeight, _pickupCollider, TriggerEntered, SetDroppedMode/SetEquippedMode |
| `Assets/Scripts/Core/Inventory/PlayerInventory.cs` | Create | Plain C# класс: owned ids, ammo dict, activeWeaponId |
| `Assets/Scripts/Core/GameContext.cs` | Modify | Добавить PlayerInventory Inventory |
| `Assets/Scripts/Core/Systems/PlayerSpawnSystem.cs` | Modify | Инициализировать инвентарь из config.player.startingInventory |
| `Assets/Scripts/Core/Systems/WeaponPickupSystem.cs` | Create | IGameSystem: dropped-weapon tracker, pickup через TriggerEntered, auto-switch |
| `Assets/Scripts/States/GameplayState.cs` | Modify | Зарегистрировать WeaponPickupSystem |
| `Assets/Scripts/Core/Systems/HealthSystem.cs` | Modify | OnEnemyDeath: weighted random drop, спавн dropped weapon |
| `Assets/Scripts/Core/Configs/GameConfig.cs` | Modify | Добавить #if UNITY_EDITOR EditorInstance (AssetDatabase + кеш) |
| `Assets/Scripts/Core/Configs/WeaponIdAttribute.cs` | Create | Marker attribute [WeaponId] |
| `Assets/Scripts/Tests/Editor/WeaponIdDrawer.cs` | Create | PropertyDrawer для [WeaponId] — выпадающий список weapon names по id |

- [x] [Task 1: Config types refactor](tasks/task-1.md)
- [x] [Task 2: WeaponView pickup mode](tasks/task-3.md)
- [x] [Task 3: Per-weapon pools](tasks/task-2.md)
- [x] [Task 4: PlayerInventory + init](tasks/task-4.md)
- [x] [Task 5: WeaponPickupSystem](tasks/task-5.md)
- [ ] [Task 6: HealthSystem weapon drop](tasks/task-6.md)
- [ ] [Task 7: WeaponId PropertyDrawer](tasks/task-7.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
