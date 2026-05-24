# 04 Object Pools

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `04` |
| `task_file` | `docs/tasks/04-object-pools.md` |
| `branch` | `feature/04-object-pools` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-24T16:42:09Z` |
| `updated_at` | `2026-05-24T16:51:24Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-24T16:51:24Z | 1 → 2 | Discovery signed off |
| 2026-05-24T16:42:09Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach

Один generic `ObjectPool<T> where T : MonoBehaviour` (pre-warm + grow). Все четыре пула агрегируются в `GamePools`-сервисе, который хранится на `GameContext`. Пулы создаются и прогреваются в `InitState.Enter()`.

### Scope

**In:**
- `ObjectPool<T>` — generic пул с pre-warm и grow
- `PoolConstants` — константы размеров (Enemies=20, Weapons=10, Projectiles=50, PowerUps=10)
- `GamePools` — сервис-агрегатор четырёх пулов
- `ProjectileView` — stub MonoBehaviour для пула снарядов
- Обновление `PrefabsConfig.projectilePrefab` с `GameObject` → `ProjectileView`
- Добавление `Pools` в `GameContext`
- Инициализация пулов в `InitState`

**Out:**
- Логика спауна/мувмента сущностей
- Назначение мешей врагам/оружию при выдаче из пула (будущие задачи)
- Логика использования снарядов

### Key Constraints

- Pre-warm + grow: N объектов предзагружаются, при нехватке создаётся новый
- Размеры pre-warm — константы в коде
- Каждый пул держит объекты в скрытом `GameObject`-контейнере (выключен, но не `DontDestroyOnLoad`)
- Префаб снаряда типизируется как `ProjectileView` для единообразия

### Files to Touch

| Action | Path |
|---|---|
| NEW | `CB-client/Assets/Scripts/Core/Pools/ObjectPool.cs` |
| NEW | `CB-client/Assets/Scripts/Core/Pools/PoolConstants.cs` |
| NEW | `CB-client/Assets/Scripts/Core/Pools/GamePools.cs` |
| NEW | `CB-client/Assets/Scripts/Entities/ProjectileView.cs` |
| MOD | `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs` |
| MOD | `CB-client/Assets/Scripts/Core/GameContext.cs` |
| MOD | `CB-client/Assets/Scripts/States/InitState.cs` |

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
