# 11 Enemy Movement System

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `11` |
| `task_file` | `docs/tasks/11-enemy-movement-system.md` |
| `branch` | `feature/11-enemy-movement-system` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-27T04:46:08Z` |
| `updated_at` | `2026-05-27T05:22:45Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-27T05:22:45Z | 1 → 2 | Discovery signed off |
| 2026-05-27T04:46:08Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach

**Strategy pattern** — `IMoveStrategy` per enemy type. Каждый тип врага реализует интерфейс с методом `GetTargetCell`. `EnemyMovementSystem` управляет beat-таймером, per-enemy `phaseOffset` (назначается случайно при спавне) и `moveCooldownTicks`.

### Scope

**В задаче:**
- `EnemyType` enum: `Pawn, Knight, Rook, Tower, Queen`
- Расширение `EnemyConfig`: `EnemyType enemyType`, `int rank`, `int moveCooldownTicks`
- `EnemyMoveState` struct: `phaseOffset`, `phaseTimer`, `cooldownTicksRemaining`
- `IMoveStrategy` интерфейс + 5 конкретных стратегий
- `EnemyMovementSystem : IGameSystem` — beat-таймер, scheduling, вызов стратегий, TickHop для врагов
- `EnemySpawnSystem`: добавить `IReadOnlyList<EnemyView> ActiveEnemies`, `System.Action<EnemyView, float> EnemySpawned` (чтобы `EnemyMovementSystem` получал phaseOffset при спавне)
- `HopAnimationSystem`: обобщить — тикать `ActiveEnemies` через `EnemySpawnSystem`
- Регистрация `EnemyMovementSystem` в `GameplayState`
- Тесты Edit Mode: каждая стратегия, collision rules, phaseOffset scheduling

**Поведение по типам:**
- **Pawn**: 4 кардинальных направления, предпочитает направление к игроку; стоп, если цель занята
- **Knight**: 8 L-клеток, игнорирует промежуточные; раннее приземление если игрок по траектории или цель занята более высокоранговым; давит врагов ниже ранга
- **Rook** (диагональ): выбирает диагональ в сторону игрока, до 5 клеток, стоп на первой заблокированной
- **Tower** (прямо): выбирает кардиналь в сторону игрока, до 5 клеток, стоп на первой заблокированной
- **Queen** (диагональ+вертикаль): 6 клеток, без прыжка, стоп на первой заблокированной

**Beat timing:**
- `EnemyMovementSystem` ведёт `_beatTimer` (0..`beatDuration`)
- Каждый враг срабатывает на `phaseOffset * beatDuration`; после хода применяется `moveCooldownTicks` задержка

**Вне задачи:**
- Визуальные микро-прыжки между шагами Ладьи/Башни (только логика позиций, анимация — следующая итерация)
- Prefab-настройка в Unity Editor

### Key Constraints

- Детерминированность: все решения через `SharedRandom`; phaseOffset назначается в момент спавна
- `HopAnimationSystem` получает `EnemySpawnSystem` через `GameContext` или параметр конструктора
- Ранги: `rank` в `EnemyConfig`; конь бьёт всё с рангом ниже, отскакивает от высшего
- Стратегии — stateless, принимают `(EnemyView enemy, GameContext ctx, System.Random rng)`

### Files to Touch

| Action | File |
|---|---|
| New | `Core/Configs/EnemyType.cs` |
| Modify | `Core/Configs/EnemyConfig.cs` |
| New | `Core/Systems/EnemyMoveState.cs` |
| New | `Core/Systems/Movement/IMoveStrategy.cs` |
| New | `Core/Systems/Movement/PawnMoveStrategy.cs` |
| New | `Core/Systems/Movement/KnightMoveStrategy.cs` |
| New | `Core/Systems/Movement/RookMoveStrategy.cs` |
| New | `Core/Systems/Movement/TowerMoveStrategy.cs` |
| New | `Core/Systems/Movement/QueenMoveStrategy.cs` |
| New | `Core/Systems/EnemyMovementSystem.cs` |
| Modify | `Core/Systems/EnemySpawnSystem.cs` |
| Modify | `Core/Systems/HopAnimationSystem.cs` |
| Modify | `States/GameplayState.cs` |
| New | `Tests/Editor/EnemyMovementTests.cs` |

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
