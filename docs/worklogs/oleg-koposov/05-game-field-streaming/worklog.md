# 05 Game Field Streaming

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `05` |
| `task_file` | `docs/tasks/05-game-field-streaming.md` |
| `branch` | `feature/05-game-field-streaming` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-24T17:44:57Z` |
| `updated_at` | `2026-05-24T17:55:02Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-24T17:55:02Z | 2 → 3 | Plan signed off |
| 2026-05-24T17:53:13Z | 1 → 2 | Discovery signed off |
| 2026-05-24T17:44:57Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach

`GameFieldSystem : IGameSystem` со скользящим окном чанков. Активные чанки хранятся в `Dictionary<Vector2Int, ChunkView>`. При смене чанка игрока окно сдвигается: выбывшие чанки очищаются и возвращаются во внутренний `ObjectPool<ChunkView>`, новые координаты загружаются из пула. `ObjectPool<T>` расширяется опциональным `Action<T> onCreate` для пост-инициализации `ChunkView`.

### Scope

**In:**
- `BoardConfig` — добавить `chunkSize` (int) и `windowRadius` (int)
- `PrefabsConfig` — добавить `ChunkView chunkPrefab`
- `ObjectPool<T>` — добавить опциональный `Action<T> onCreate` callback
- `ChunkView` — MonoBehaviour с `Initialize()`, `Setup()`, `Clear()`
- `ChunkCoordConverter` — статический хелпер world ↔ chunk
- `GameFieldSystem : IGameSystem` — внутренний пул, скользящее окно, `OnPlayerChunkChanged()`
- `GameplayState` — создание и регистрация `GameFieldSystem`

**Out:**
- Реальное чтение позиции игрока
- Спавн врагов/оружия/павер-апов в чанках
- Визуальный контент тайлов (меши назначаются, но наполнение сцены — будущая задача)

### Key Constraints

- Размер чанка (`chunkSize`) и радиус окна (`windowRadius`) задаются в `BoardConfig`
- `ChunkView` создаёт тайлы динамически при первой инициализации (lazy, через `onCreate` callback)
- Интеграция с игроком — через публичный метод `OnPlayerChunkChanged(Vector2Int)` как точка подключения будущих систем

### Files to Touch

| Action | Path |
|---|---|
| MOD | `CB-client/Assets/Scripts/Core/Configs/BoardConfig.cs` |
| MOD | `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs` |
| MOD | `CB-client/Assets/Scripts/Core/Pools/ObjectPool.cs` |
| NEW | `CB-client/Assets/Scripts/Entities/ChunkView.cs` |
| NEW | `CB-client/Assets/Scripts/Core/GameField/ChunkCoordConverter.cs` |
| NEW | `CB-client/Assets/Scripts/Core/GameField/GameFieldSystem.cs` |
| MOD | `CB-client/Assets/Scripts/States/GameplayState.cs` |

## Tasks

**Goal:** Создать систему бесконечного игрового поля на основе скользящего окна чанков. Чанки — матрицы тайлов с логическими координатами `Vector2Int`; при смене чанка игрока окно сдвигается, выбывшие чанки очищаются и переиспользуются через пул. Интеграция с движением игрока и спауном сущностей — за пределами этой задачи.

**Architecture:** `GameFieldSystem : IGameSystem` хранит `Dictionary<Vector2Int, ChunkView>` и внутренний `ObjectPool<ChunkView>`. Конфиг (размер чанка, радиус окна) читается из `BoardConfig`. `ChunkView` создаёт тайлы `BoardTileView` динамически при первой инициализации через `onCreate`-callback пула. `GameplayState` создаёт и регистрирует систему.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Core/Configs/BoardConfig.cs` | Modify | Добавить chunkSize и windowRadius |
| `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs` | Modify | Добавить ChunkView chunkPrefab |
| `CB-client/Assets/Scripts/Core/Pools/ObjectPool.cs` | Modify | Добавить опциональный onCreate callback |
| `CB-client/Assets/Scripts/Entities/ChunkView.cs` | Create | MonoBehaviour чанка: Initialize/Setup/Clear |
| `CB-client/Assets/Scripts/Core/GameField/ChunkCoordConverter.cs` | Create | Статический хелпер world ↔ chunk coords |
| `CB-client/Assets/Scripts/Core/GameField/GameFieldSystem.cs` | Create | IGameSystem со скользящим окном чанков |
| `CB-client/Assets/Scripts/States/GameplayState.cs` | Modify | Создать и зарегистрировать GameFieldSystem |

- [x] [Task 1: Extend configs and ObjectPool](tasks/task-1.md)
- [x] [Task 2: ChunkView and ChunkCoordConverter](tasks/task-2.md)
- [ ] [Task 3: GameFieldSystem and GameplayState wiring](tasks/task-3.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
