# 11 Enemy Movement System

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `11` |
| `task_file` | `docs/tasks/11-enemy-movement-system.md` |
| `branch` | `feature/11-enemy-movement-system` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-27T04:46:08Z` |
| `updated_at` | `2026-05-27T06:13:58Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-27T06:13:58Z | 2 → 3 | Plan signed off |
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

**Goal:** Implement a full enemy movement system for all 5 chess-like enemy types (Pawn, Knight, Rook, Tower, Queen). Each enemy type has its own movement strategy and acts on a beat timer with a random per-enemy phase offset. The system integrates with the existing `GridMovementSystem`, extends `EnemySpawnSystem` to expose active enemies, and generalises `HopAnimationSystem` to animate all enemies.

**Architecture:** Strategy pattern — `IMoveStrategy` per `EnemyType`; `EnemyMovementSystem` maintains a beat timer and a `Dictionary<EnemyView, EnemyMoveState>` for phase/cooldown tracking. `EnemyConfig` is extended with `enemyType`, `rank`, and `moveCooldownTicks`. All randomness routes through `GameContext.SharedRandom`.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `Core/Configs/EnemyType.cs` | Create | `EnemyType` enum: Pawn/Knight/Rook/Tower/Queen |
| `Core/Configs/EnemyConfig.cs` | Modify | Add `enemyType`, `rank`, `moveCooldownTicks` fields |
| `Core/Systems/EnemyMoveState.cs` | Create | Per-enemy state struct: phaseOffset, phaseTimer, cooldownTicksLeft |
| `Core/Systems/Movement/IMoveStrategy.cs` | Create | Interface: `GetMoveDirection(enemy, ctx, rng) → Vector2Int?` |
| `Core/Systems/Movement/LinearStrategy.cs` | Create | Shared helper for Rook/Tower/Queen: walk up to N cells, stop at blocker |
| `Core/Systems/Movement/PawnMoveStrategy.cs` | Create | Prefer direction toward player, skip occupied |
| `Core/Systems/Movement/KnightMoveStrategy.cs` | Create | 8 L-shapes, early-landing, rank-based crush |
| `Core/Systems/Movement/RookMoveStrategy.cs` | Create | Diagonal toward player, up to 5 cells |
| `Core/Systems/Movement/TowerMoveStrategy.cs` | Create | Cardinal toward player, up to 5 cells |
| `Core/Systems/Movement/QueenMoveStrategy.cs` | Create | All 8 dirs toward player, up to 6 cells |
| `Core/Systems/EnemyMovementSystem.cs` | Create | Beat timer, strategy dispatch, spawn/death callbacks |
| `Core/Systems/EnemySpawnSystem.cs` | Modify | Add `ActiveEnemies` property + `EnemySpawned` callback |
| `Core/Systems/HopAnimationSystem.cs` | Modify | Accept `EnemySpawnSystem`; tick all active enemies |
| `States/GameplayState.cs` | Modify | Register `EnemyMovementSystem`; wire all callbacks |
| `Tests/Editor/EnemyMovementTests.cs` | Test | Edit Mode tests for all 5 strategies + LinearStrategy |
| `Core/GameContext.cs` | Modify | Add internal test-only constructor (OccupancyMap overload) |

- [x] [Task 1: EnemyType + EnemyConfig extensions](tasks/task-1.md)
- [x] [Task 2: IMoveStrategy + EnemyMoveState + 5 strategies](tasks/task-2.md)
- [x] [Task 3: EnemyMovementSystem + EnemySpawnSystem + HopAnimationSystem + GameplayState](tasks/task-3.md)
- [x] [Task 4: Edit Mode tests](tasks/task-4.md)

## What we did

- Added `EnemyType` enum (Pawn, Knight, Rook, Tower, Queen) and extended enemy configuration with type, rank, and cooldown fields.
- Introduced a strategy interface so each enemy type independently decides its move direction each beat, keeping movement logic decoupled and testable.
- Pawn picks the best of 4 cardinal directions toward the player; Knight jumps in L-shapes with early-landing and rank-based collision rules; Rook/Tower/Queen slide up to 5–6 cells using a shared linear helper that stops at the first blocker.
- Added a new `EnemyMovementSystem` that drives a beat timer, assigns each spawned enemy a random phase offset, and dispatches strategy calls with cooldown tracking.
- `EnemySpawnSystem` now exposes a read-only active-enemy list and a spawn callback so downstream systems react to new enemies.
- `HopAnimationSystem` was generalised to tick hop animations for all active enemies, not just the player.
- All spawn, death, and movement callbacks are wired in `GameplayState` using multicast delegates (`+=`).
- Added 11 Edit Mode tests covering every strategy's direction selection, blocking behaviour, and beat threshold wrapping logic.
