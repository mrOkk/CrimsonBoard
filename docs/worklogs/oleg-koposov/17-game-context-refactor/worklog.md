# 17 Game Context Refactor

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `17` |
| `task_file` | `docs/tasks/17-game-context-refactor.md` |
| `branch` | `feature/17-game-context-refactor` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-06-05T15:31:25Z` |
| `updated_at` | `2026-06-05T15:38:48Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-06-05T15:38:48Z | 1 → 2 | Discovery signed off |
| 2026-06-05T15:31:25Z | — → 1 | Created by wf-start |

## Discovery

### Scope

Исправить `GameContext` в соответствии с AGENTS.md: убрать ссылки на системы, создать `GameBoard` как центр данных доски.

**In scope:**
- Создать класс `GameBoard` (новые данные: активные противники, граничные клетки для спавна)
- Убрать `GameFieldSystem` и `EnemySpawnSystem` из `GameContext`
- Обновить потребителей: `EnemySpawnSystem`, `WeaponUsageSystem`, `GameOverState`, `TapToStartState`, `GameplayState`

**Out of scope:**
- Перенос `OccupancyMap` в `GameBoard` (оставить в `GameContext`)
- Реализация хранения позиций оружия и power-ups (пока не реализовано в системе)
- Удаление `GameContext.Instance` синглтона (отдельная задача)

### Chosen Approach

**GameBoard as data entity:**
- `GameBoard` содержит `IReadOnlyList<EnemyView> ActiveEnemies` и метод `GetBorderTiles()`
- `EnemySpawnSystem` управляет списком врагов в `GameBoard` и читает из него данные для спавна
- Системы получают `GameBoard` через конструктор, а не через `_context.System`
- `GameplayState` создаёт `GameBoard` и передаёт его в `GameContext` и системы

**Rationale:**
- Решает нарушение AGENTS.md (системы не хранятся в контексте)
- Минимальные изменения: только 7 файлов
- Расширяемо: позже можно добавить позиции оружия/power-ups в `GameBoard`

### Key Constraints

- `GameplayState` создаёт системы и соединяет их callback'ами — нужно передавать `GameBoard` при создании
- `TapToStartState` создаёт `GameFieldSystem` — нужен `GameBoard` для получения граничных клеток
- `GameOverState` должен иметь доступ к врагам для dissolve-анимации

### Files to Touch

- `Core/GameBoard.cs` (новый)
- `Core/GameContext.cs`
- `Core/Systems/EnemySpawnSystem.cs`
- `Core/Systems/WeaponUsageSystem.cs`
- `States/TapToStartState.cs`
- `States/GameOverState.cs`
- `States/GameplayState.cs`


## Tasks

**Goal:** Refactor `GameContext` to comply with AGENTS.md — remove system references and introduce a `GameBoard` data entity that holds board-level data (active enemies, border tiles, weapon positions, power-up positions).

**Architecture:**
`GameBoard` is a plain C# class (not a MonoBehaviour, not a system). It wraps `GameFieldSystem` (stored internally) for border-tile queries and **owns** the `_activeEnemies` list. Systems receive `GameBoard` via constructor — never reach into other systems through `_context`. `GameplayState` accesses the underlying `GameFieldSystem` through `GameBoard.FieldSystem` for runner registration only.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `Core/GameBoard.cs` | Create | Data entity: active enemies, border tiles, dropped weapons, power-ups |
| `Core/GameContext.cs` | Modify | Add `Board` property, remove `GameFieldSystem` and `EnemySpawnSystem` |
| `States/TapToStartState.cs` | Modify | Create `GameBoard`, wire to context |
| `Core/Systems/EnemySpawnSystem.cs` | Modify | Read/write enemies via `GameBoard` |
| `Core/Systems/HopAnimationSystem.cs` | Modify | Read enemies from `GameBoard` instead of `EnemySpawnSystem` |
| `Core/Systems/WeaponUsageSystem.cs` | Modify | Read enemies from `_context.Board` |
| `States/GameplayState.cs` | Modify | Pass `GameBoard` to systems, register field system via `Board.FieldSystem` |
| `States/GameOverState.cs` | Modify | Read enemies from `_context.Board` |

- [x] [Task 1: Create GameBoard data class](tasks/task-1.md)
- [x] [Task 2: Migrate GameFieldSystem out of GameContext](tasks/task-2.md)
- [x] [Task 3: Migrate EnemySpawnSystem out of GameContext](tasks/task-3.md)


## What we did

Рефакторинг GameContext в соответствии с AGENTS.md:
- Создан класс `GameBoard` как data entity для хранения данных доски
- Убраны ссылки на системы (`GameFieldSystem`, `EnemySpawnSystem`) из `GameContext`
- Активные противники теперь хранятся в `GameBoard.ActiveEnemies`
- Системы получают `GameBoard` через конструктор (зависимости систем)
- Обновлены все consumers: `TapToStartState`, `GameplayState`, `GameOverState`, `WeaponUsageSystem`, `HopAnimationSystem`
