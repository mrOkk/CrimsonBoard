# 09 User Input System

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `09` |
| `task_file` | `docs/tasks/09-user-input-system.md` |
| `branch` | `feature/09-user-input-system` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-26T16:06:00Z` |
| `updated_at` | `2026-05-26T18:02:05Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-26T18:02:05Z | 2 → 3 | Plan signed off |
| 2026-05-26T17:56:30Z | 1 → 2 | Discovery signed off |
| 2026-05-26T16:06:00Z | — → 1 | Created by wf-start |

## Discovery

### Existing state

`PlayerMovementSystem` already owns `InputSystem_Actions`, reads `Player.Move`, snaps to 8 directions, and uses a cooldown tied to `beatDuration / movesPerBeat`. There is no input delay, no input buffer, and no movement animation — `GridMovementSystem.TryMove` snaps `entity.transform.position` immediately. The `InputSystem_Actions` asset also exposes `Player.Attack` (unused so far).

### Chosen approach

**Two new systems: `PlayerInputSystem` + `HopAnimationSystem`. Communication via `InputState` on `GameContext`.**

#### Input layer (`PlayerInputSystem`)

- New `IGameSystem` that exclusively owns `InputSystem_Actions`. Ticks *before* `PlayerMovementSystem`.
- **Movement input delay** (first move only): on the 0→non-zero transition, starts a settling timer (`movementInputDelay` from `PlayerConfig`). During this window the direction is continuously updated to allow diagonal refinement. Once the delay expires or the button is released, the last tracked direction is emitted as `InputState.MoveCommand`. After the first move, subsequent moves use only the cooldown — no further delay.
- **Movement buffer**: on release, keeps the last direction in `InputState.MoveCommand` for `inputBufferWindow` seconds, then clears it.
- **Shoot buffer**: on `Player.Attack` press, sets `InputState.ShootCommandBuffered = true`; clears after `inputBufferWindow` seconds.
- `PlayerMovementSystem` refactored to consume `InputState.MoveCommand` and no longer references `InputSystem_Actions`.

#### Movement animation (`HopAnimationSystem` + `EntityView.StartHop/TickHop`)

- `EntityView` gains `StartHop(dir, from, to, HopConfig)` and `TickHop(float dt)` methods.
- `GridMovementSystem.TryMove` — instead of snapping `entity.transform.position`, calls `entity.StartHop(...)` with the source/target world positions. Logical state (`CurrentCell`, `OccupancyMap`) still updates immediately.
- Hop animation plays in two phases driven by `TickHop`:
  1. **Windup**: brief visual displacement opposite to the move direction.
  2. **Hop**: parabolic arc from source to target cell.
- `HopAnimationSystem` is a new `IGameSystem` registered in `GameplaySystemRunner`; for now it ticks the player only (enemy support is a future task — `StartHop/TickHop` are on `EntityView` ready for reuse).
- Parameters (`hopHeight`, `windupAmplitude`, `windupDuration`, `hopDuration`) live in a new `HopConfig`, added to `GameConfig`.

#### Config & shared state

- `PlayerConfig`: add `movementInputDelay` + `inputBufferWindow`.
- `GameConfig`: add `public HopConfig hop`.
- `GameContext`: add `public InputState InputState`.

### Scope

**In:**
- `PlayerConfig` — add `movementInputDelay`, `inputBufferWindow`
- `HopConfig` (new) + added to `GameConfig`
- `InputState` (new) + added to `GameContext`
- `PlayerInputSystem` (new)
- `HopAnimationSystem` (new)
- `EntityView` — add `StartHop` + `TickHop`
- `GridMovementSystem` — replace snap with `StartHop` call
- `PlayerMovementSystem` — remove input reading, consume `InputState.MoveCommand`
- `GameplayState` — register new systems in correct order

**Out:**
- Actual shoot / combat logic — only `InputState.ShootCommandBuffered` is set
- Enemy hop animation wiring (future task — `StartHop/TickHop` are prepared but not called for enemies)
- Unit tests (not required)

### Key constraints

- System registration order: `PlayerInputSystem` → `PlayerMovementSystem` / enemy systems → `HopAnimationSystem`.
- Logical position (`CurrentCell`, `OccupancyMap`) updates immediately on move; only the *visual* position animates.
- Timing is frame-delta-based (`Time.deltaTime` via `IGameSystem.Tick`).
- Input delay applies **only to the first move** after a fresh press; subsequent moves while held are gated solely by the movement cooldown.

### Files to touch

| File | Change |
|---|---|
| `Core/Configs/PlayerConfig.cs` | Add `movementInputDelay`, `inputBufferWindow` |
| `Core/Configs/HopConfig.cs` | New |
| `Core/Configs/GameConfig.cs` | Add `public HopConfig hop` |
| `Core/InputState.cs` | New |
| `Core/GameContext.cs` | Add `public InputState InputState` |
| `Core/Systems/PlayerInputSystem.cs` | New |
| `Core/Systems/HopAnimationSystem.cs` | New |
| `Core/Systems/PlayerMovementSystem.cs` | Remove input; consume `InputState.MoveCommand` |
| `Core/Systems/GridMovementSystem.cs` | Replace snap-position with `entity.StartHop(...)` |
| `Entities/EntityView.cs` | Add `StartHop` + `TickHop` |
| `States/GameplayState.cs` | Register new systems |

## Tasks

**Goal:** Implement the second iteration of the player input system: separate input reading from movement logic via a new `PlayerInputSystem` that handles movement input delay (first-move only, diagonal settling), movement buffer (persists command briefly after release), and shoot buffer. Add a code-driven hop animation (windup offset + parabolic arc) reusable for entities, driven by a new `HopAnimationSystem`. Wire everything through `GameContext.InputState` and new config fields.

**Architecture:** `PlayerInputSystem` (new `IGameSystem`) owns `InputSystem_Actions`, writes to `GameContext.InputState` each tick, and is registered before `PlayerMovementSystem`. `PlayerMovementSystem` is stripped of input concerns and reads `InputState.MoveCommand`. `EntityView` gains `StartHop`/`TickHop` methods; `GridMovementSystem` calls `StartHop` instead of snapping position; `HopAnimationSystem` ticks the animation each frame.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `Core/Configs/HopConfig.cs` | Create | Hop animation parameters |
| `Core/Configs/PlayerConfig.cs` | Modify | Add `movementInputDelay`, `inputBufferWindow` |
| `Core/Configs/GameConfig.cs` | Modify | Add `public HopConfig hop` |
| `Core/InputState.cs` | Create | Shared input command struct updated by `PlayerInputSystem` |
| `Core/GameContext.cs` | Modify | Expose `InputState InputState` property |
| `Core/Systems/PlayerInputSystem.cs` | Create | Reads `InputSystem_Actions`, manages delay + buffer |
| `Core/Systems/HopAnimationSystem.cs` | Create | Ticks hop animation on registered entities |
| `Core/Systems/PlayerMovementSystem.cs` | Modify | Remove input; consume `InputState.MoveCommand` |
| `Core/Systems/GridMovementSystem.cs` | Modify | Call `entity.StartHop(...)` instead of snap |
| `Entities/EntityView.cs` | Modify | Add `StartHop` + `TickHop` |
| `States/GameplayState.cs` | Modify | Register `PlayerInputSystem` and `HopAnimationSystem` |

- [ ] [Task 1: Configs and InputState scaffolding](tasks/task-1.md)
- [ ] [Task 2: EntityView hop animation + GridMovementSystem](tasks/task-2.md)
- [ ] [Task 3: HopAnimationSystem](tasks/task-3.md)
- [ ] [Task 4: PlayerInputSystem](tasks/task-4.md)
- [ ] [Task 5: Refactor PlayerMovementSystem + wire GameplayState](tasks/task-5.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
