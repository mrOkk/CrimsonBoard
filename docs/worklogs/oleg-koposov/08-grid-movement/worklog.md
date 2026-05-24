# 08 Grid Movement

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `08` |
| `task_file` | `docs/tasks/08-grid-movement.md` |
| `branch` | `feature/08-grid-movement` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-24T19:14:08Z` |
| `updated_at` | `2026-05-24T19:26:19Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-24T19:26:19Z | 2 → 3 | Plan signed off |
| 2026-05-24T19:24:34Z | 1 → 2 | Discovery signed off |
| 2026-05-24T19:14:08Z | — → 1 | Created by wf-start |

## Discovery

**Подход:** Два новых IGameSystem: `GridMovementSystem` (логика TryMove + интеграция с OccupancyMap/HealthSystem) и `PlayerMovementSystem` (ввод через InputSystem_Actions, 8 направлений, задержка = `beatDuration / movesPerBeat`).

**Архитектура:**
- `MoveResult` — enum: `Moved`, `Blocked`, `Combat`
- `GridMovementSystem : IGameSystem` — `TryMove(EntityView entity, Vector2Int dir) → MoveResult`; проверяет OccupancyMap; при входе врага в клетку игрока вызывает `HealthSystem.ApplyDamageToPlayer`; при успехе обновляет OccupancyMap + transform + EntityView.CurrentCell
- `PlayerMovementSystem : IGameSystem` — подписывается на `InputSystem_Actions.Player.Move`; преобразует Vector2 в 8-directional grid dir (round to nearest); применяет кулдаун `beatDuration / movesPerBeat`; при успешном ходе поворачивает `_directionIndicator` на PlayerView
- `ChunkCoordConverter` расширяется методами `WorldToTile(Vector3, BoardConfig) → Vector2Int` и `TileToWorld(Vector2Int, BoardConfig) → Vector3` (центр клетки)
- `EntityView` получает `Vector2Int CurrentCell` (публичное поле/property, выставляется снаружи)
- `PlayerView` получает `[SerializeField] Transform _directionIndicator` + публичный accessor
- `PlayerSpawnSystem` обновляется: после спавна регистрирует игрока в OccupancyMap и выставляет `CurrentCell`
- `HealthSystem.ApplyDamageToPlayer` — убирается TODO, реальный move transform через TileToWorld

**Scope in:** GridMovementSystem, PlayerMovementSystem (WASD + gamepad stick, задержка, 8 dir, indicator), EntityView.CurrentCell, PlayerView.DirectionIndicator, ChunkCoordConverter (tile methods), PlayerSpawnSystem OccupancyMap wiring, HealthSystem knockback transform  
**Scope out:** движение врагов (AI-логика), визуальная интерполяция, beat system, touch input, WeaponView rotation (только параметр в конфиге уже есть)

**Ключевые ограничения:**
- Диагональное движение разрешено (8 направлений)
- Задержка между ходами = `beatDuration / movesPerBeat` (из конфига)
- Детерминизм: TryMove не зависит от порядка Tick, только от состояния OccupancyMap в момент вызова
- InputSystem_Actions.cs уже сгенерирован в `Assets/Settings/Input/`

**Файлы:**
- `Core/GameField/ChunkCoordConverter.cs` — Modify: добавить WorldToTile/TileToWorld
- `Entities/EntityView.cs` — Modify: добавить CurrentCell
- `Entities/PlayerView.cs` — Modify: добавить DirectionIndicator
- `Core/Systems/GridMovementSystem.cs` — Create
- `Core/Systems/PlayerMovementSystem.cs` — Create
- `Core/Systems/PlayerSpawnSystem.cs` — Modify: OccupancyMap.Register + CurrentCell
- `Core/Systems/HealthSystem.cs` — Modify: реальный move transform
- `States/GameplayState.cs` — Modify: регистрация систем

## Tasks

**Goal:** Реализовать клеточное движение для игрока: центральный `GridMovementSystem` проверяет занятость клеток и разрешает конфликты с HealthSystem, `PlayerMovementSystem` читает WASD/стик через InputSystem_Actions, поддерживает 8 направлений с задержкой по конфигу, поворачивает стрелочный индикатор направления.

**Architecture:** `GridMovementSystem.TryMove` — единственная точка входа для логического перемещения любой сущности; обновляет OccupancyMap, transform и CurrentCell, при столкновении вызывает HealthSystem. `PlayerMovementSystem` реализует ввод + кулдаун и делегирует TryMove. Tile-координаты (не chunk-уровень) добавляются в ChunkCoordConverter.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `Core/GameField/ChunkCoordConverter.cs` | Modify | добавить WorldToTile/TileToWorld |
| `Entities/EntityView.cs` | Modify | добавить CurrentCell |
| `Entities/PlayerView.cs` | Modify | добавить DirectionIndicator |
| `Core/Systems/MoveResult.cs` | Create | enum Moved/Blocked/Combat |
| `Core/Systems/GridMovementSystem.cs` | Create | TryMove + OccupancyMap + HealthSystem |
| `Core/Systems/PlayerMovementSystem.cs` | Create | InputSystem_Actions, 8 dir, cooldown, indicator |
| `Core/Systems/PlayerSpawnSystem.cs` | Modify | OccupancyMap.Register + CurrentCell при спавне |
| `Core/Systems/HealthSystem.cs` | Modify | реальный move transform через TileToWorld |
| `States/GameplayState.cs` | Modify | регистрация GridMovementSystem + PlayerMovementSystem |

- [x] [Task 1: Foundation — tile coords, EntityView.CurrentCell, PlayerView.DirectionIndicator](tasks/task-1.md)
- [ ] [Task 2: GridMovementSystem — MoveResult, TryMove, OccupancyMap, HealthSystem](tasks/task-2.md)
- [ ] [Task 3: PlayerMovementSystem — input, cooldown, 8-dir, direction indicator](tasks/task-3.md)
- [ ] [Task 4: Wiring — PlayerSpawnSystem, HealthSystem knockback, GameplayState](tasks/task-4.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
