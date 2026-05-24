# 04 Object Pools

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `04` |
| `task_file` | `docs/tasks/04-object-pools.md` |
| `branch` | `feature/04-object-pools` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `done`|
| `created_at` | `2026-05-24T16:42:09Z` |
| `updated_at` | `2026-05-24T17:31:22Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-24T17:31:22Z | active → done | Implemented by wf-implement |
| 2026-05-24T17:28:53Z | 2 → 3 | Plan signed off |
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

**Goal:** Создать систему пулов объектов для всех игровых сущностей (враги, оружие, снаряды, павер-апы) с инициализацией на старте игры. Каждый пул прогревается заранее и умеет расти при нехватке объектов.

**Architecture:** Один generic `ObjectPool<T> where T : MonoBehaviour` хранит объекты в скрытом `GameObject`-контейнере и предоставляет API `Get()`/`Return()`. `GamePools` агрегирует четыре типизированных пула и хранится на `GameContext.Pools`. Инициализация происходит в `InitState.Enter()`.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Core/Pools/ObjectPool.cs` | Create | Generic пул с pre-warm и grow |
| `CB-client/Assets/Scripts/Core/Pools/PoolConstants.cs` | Create | Константы размеров pre-warm |
| `CB-client/Assets/Scripts/Core/Pools/GamePools.cs` | Create | Агрегатор четырёх пулов |
| `CB-client/Assets/Scripts/Entities/ProjectileView.cs` | Create | Stub MonoBehaviour для снарядов |
| `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs` | Modify | projectilePrefab: GameObject → ProjectileView |
| `CB-client/Assets/Scripts/Core/GameContext.cs` | Modify | Добавить свойство Pools |
| `CB-client/Assets/Scripts/States/InitState.cs` | Modify | Создать GamePools и прогреть |

- [x] [Task 1: Core pool infrastructure](tasks/task-1.md)
- [x] [Task 2: Wire pools into GameContext and InitState](tasks/task-2.md)

## What we did

Добавлена система пулов объектов для четырёх игровых сущностей: враги, оружие, снаряды и павер-апы.

- Реализован общий механизм пула: объекты предзагружаются при старте (20/10/50/10 штук соответственно) и автоматически создаются при нехватке. Неиспользуемые объекты хранятся в скрытых контейнерах сцены.
- Все пулы инициализируются на стадии запуска игры, до перехода к экрану «Tap to Start».
- Добавлен stub-скрипт для снарядов (`ProjectileView`), выровнен тип поля префаба снаряда с остальными сущностями.
- Пулы доступны через `GameContext.Pools` и готовы к использованию в логике спауна.
