# 10 Enemy Spawn Iteration 1

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `10` |
| `task_file` | `docs/tasks/10-enemy-spawn-iteration-1.md` |
| `branch` | `feature/10-enemy-spawn-iteration-1` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-27T04:26:29Z` |
| `updated_at` | `2026-05-27T04:32:46Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-27T04:32:46Z | 1 → 2 | Discovery signed off |
| 2026-05-27T04:26:29Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach

**Inline `SpawnConfig` в `GameConfig`** (Подход 1). Следует существующему паттерну — все конфиги как `[Serializable]` классы, вложенные в единый `GameConfig` ScriptableObject. Добавляем поле `public SpawnConfig spawn;`.

### Scope

**В задаче:**
- `SpawnConfig` (`waveInterval`, `WaveConfig[]`) + `WaveConfig` (`maxAliveEnemies`, `spawnFrequencyRangeSec`, `spawnBatchSizeRange`, `EnemySpawnEntry[]`) + `EnemySpawnEntry` (`enemyId`, `weight`)
- `GameConfig.spawn` — новое поле
- `System.Random SharedRandom` в `GameContext` (seed из `GameConfig` или фиксированный в InitState)
- `GameFieldSystem.GetBorderTiles()` — возвращает граничные тайлы активного окна чанков
- `EnemySpawnSystem : IGameSystem` — управление волнами, таймерами, пул-спавн через `GamePools.Enemies`
- Регистрация `EnemySpawnSystem` в `GameplayState`
- Edit Mode тесты: граничные тайлы, взвешенный выбор врага, переходы волн

**Вне задачи:**
- Движение врагов (`GridMovementSystem.Tick`) — отдельная задача
- Prefab-настройка в Unity Editor (только код и структура конфигов)

### Key Constraints

- Детерминированность: `System.Random` с сидом, добавить `seed` (int) в `GameConfig` (или `SpawnConfig`)
- Граница спавна = крайние тайлы активного окна (`windowRadius` в `BoardConfig`)
- Не спавнить в занятые/заблокированные клетки; при неудаче — повтор из доступных граничных клеток
- Последняя волна повторяется до конца забега
- `max alive enemies` проверяется перед каждым батч-спавном

### Files to Touch

| Action | File |
|---|---|
| Modify | `CB-client/Assets/Scripts/Core/Configs/GameConfig.cs` |
| New | `CB-client/Assets/Scripts/Core/Configs/SpawnConfig.cs` |
| New | `CB-client/Assets/Scripts/Core/Configs/WaveConfig.cs` |
| New | `CB-client/Assets/Scripts/Core/Configs/EnemySpawnEntry.cs` |
| Modify | `CB-client/Assets/Scripts/Core/GameContext.cs` |
| Modify | `CB-client/Assets/Scripts/Core/GameField/GameFieldSystem.cs` |
| New | `CB-client/Assets/Scripts/Core/Systems/EnemySpawnSystem.cs` |
| Modify | `CB-client/Assets/Scripts/States/GameplayState.cs` |
| New | `CB-client/Assets/Scripts/Tests/EditMode/EnemySpawnTests.cs` (+ asmdef) |

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
