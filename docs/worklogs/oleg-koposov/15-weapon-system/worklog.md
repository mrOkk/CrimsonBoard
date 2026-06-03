# 15 Weapon System

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `15` |
| `task_file` | `docs/tasks/15-weapon-system.md` |
| `branch` | `feature/15-weapon-system` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-06-03T08:44:20Z` |
| `updated_at` | `2026-06-03T09:08:24Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
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

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
