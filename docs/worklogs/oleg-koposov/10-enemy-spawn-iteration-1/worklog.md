# 10 Enemy Spawn Iteration 1

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `10` |
| `task_file` | `docs/tasks/10-enemy-spawn-iteration-1.md` |
| `branch` | `feature/10-enemy-spawn-iteration-1` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `done`|
| `created_at` | `2026-05-27T04:26:29Z` |
| `updated_at` | `2026-05-27T04:43:54Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-27T04:43:54Z | active → done | Implemented by wf-implement |
| 2026-05-27T04:36:24Z | 2 → 3 | Plan signed off |
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

**Goal:** Реализовать базовую волновую систему спавна врагов (`EnemySpawnSystem`) с детерминированным выбором граничных тайлов и типов врагов по весам. Добавить конфиги волн (`SpawnConfig`, `WaveConfig`, `EnemySpawnEntry`) в `GameConfig`. Покрыть ключевые алгоритмы Edit Mode тестами.

**Architecture:** Следуем паттерну `IGameSystem` — `EnemySpawnSystem` регистрируется в `GameplaySystemRunner` из `GameplayState`. Весь общий стейт (сид, RNG, пул врагов) живёт в `GameContext`. Вычисление граничных тайлов вынесено в статический метод для тестируемости.

**File structure:**

| Path | Type | Purpose |
|---|---|---|
| `Core/Configs/EnemySpawnEntry.cs` | Create | `[Serializable]` запись типа врага: `enemyId` + `weight` |
| `Core/Configs/WaveConfig.cs` | Create | `[Serializable]` параметры одной волны |
| `Core/Configs/SpawnConfig.cs` | Create | `[Serializable]` верхний конфиг спавна: `waveInterval`, `randomSeed`, `waves[]` |
| `Core/Configs/GameConfig.cs` | Modify | Добавить поле `public SpawnConfig spawn;` |
| `Core/GameContext.cs` | Modify | Добавить `System.Random SharedRandom`, инициализировать из `config.spawn.randomSeed` |
| `Core/GameField/GameFieldSystem.cs` | Modify | Добавить `GetBorderTiles()` + `ComputeBorderTiles()` static helper |
| `Core/Systems/EnemySpawnSystem.cs` | Create | Волновые таймеры, батч-спавн через пул, `OnEnemyDied` callback |
| `Core/Systems/HealthSystem.cs` | Modify | Добавить `EnemyDeathCallback` → вызов при смерти врага |
| `States/GameplayState.cs` | Modify | Регистрация `EnemySpawnSystem`, подключение `EnemyDeathCallback` |
| `Tests/Editor/CrimsonBoard.Tests.EditMode.asmdef` | Create | Test assembly definition |
| `Tests/Editor/EnemySpawnTests.cs` | Create | Тесты граничных тайлов, весов, таймера волн |

- [x] [Task 1: Spawn config classes](tasks/task-1.md)
- [x] [Task 2: SharedRandom + GetBorderTiles](tasks/task-2.md)
- [x] [Task 3: EnemySpawnSystem](tasks/task-3.md)
- [x] [Task 4: Edit Mode Tests](tasks/task-4.md)

## What we did

Добавлена система волнового спавна врагов. Новые конфиги (`SpawnConfig`, `WaveConfig`, `EnemySpawnEntry`) встроены в `GameConfig` — настраиваются прямо в Inspector без дополнительных ассетов. `GameContext` получил детерминированный `System.Random` с сидом из конфига. `GameFieldSystem` теперь умеет возвращать список граничных тайлов активного окна доски. Новая система `EnemySpawnSystem` управляет циклом волн по таймеру (последняя волна не меняется), каждый тик спавнит батч врагов на незанятых граничных клетках с весовым выбором типа. `HealthSystem` оповещает систему о смерти врага через колбек. Добавлены 8 Edit Mode тестов, покрывающих вычисление границы, детерминированный выбор по весам и механику смены волн.
