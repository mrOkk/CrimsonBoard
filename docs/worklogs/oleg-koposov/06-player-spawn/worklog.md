# 06 Player Spawn

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `06` |
| `task_file` | `docs/tasks/06-player-spawn.md` |
| `branch` | `feature/06-player-spawn` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-24T18:01:32Z` |
| `updated_at` | `2026-05-24T18:34:00Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-24T18:34:00Z | 2 → 3 | Plan signed off |
| 2026-05-24T18:31:48Z | 1 → 2 | Discovery signed off |
| 2026-05-24T18:01:32Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach

`PlayerSpawnSystem : IGameSystem` в `Core/Systems/`. `Initialize()` обрабатывает оба сценария: первый спавн (создаёт инстанс) и ре-спавн при новом раунде (перемещает существующий объект). Стартовая позиция — центр чанка (0,0). Ссылка на игрока хранится в `GameContext.Player`.

### Scope

**In:**
- `PlayerSpawnSystem` — IGameSystem, Instantiate при первом спавне, reposition при ре-спавне
- `GameContext` — добавить `PlayerView Player { get; set; }`
- `GameplayState` — зарегистрировать `PlayerSpawnSystem`

**Out:**
- Движение игрока, получение урона
- Death/respawn-события и UI
- Интеграция с entity-компонентами (будущая задача)

### Key Constraints

- Игрок — одиночка, не пулируется
- При ре-спавне — reposition (не Destroy + Instantiate)
- Стартовая позиция: центр чанка (0,0) = `new Vector3(chunkSize * tileSize.x / 2, 0, chunkSize * tileSize.y / 2)`
- Меш назначается из `PlayerConfig.mesh` через `PlayerView.MeshFilter.sharedMesh`

### Files to Touch

| Action | Path |
|---|---|
| NEW | `CB-client/Assets/Scripts/Core/Systems/PlayerSpawnSystem.cs` |
| MOD | `CB-client/Assets/Scripts/Core/GameContext.cs` |
| MOD | `CB-client/Assets/Scripts/States/GameplayState.cs` |

## Tasks

**Goal:** Создать систему спавна игрока, которая при старте геймплея размещает игрока в центре стартового чанка и сохраняет ссылку на него в контексте. При ре-спавне существующий объект перемещается в стартовую позицию без пересоздания.

**Architecture:** `PlayerSpawnSystem : IGameSystem` в `Core/Systems/` обрабатывает оба сценария — первичный спавн (`Instantiate`) и ре-спавн (reposition). Позиция вычисляется как центр чанка (0,0) из параметров `BoardConfig`. `GameContext.Player` хранит ссылку на активного игрока. Система регистрируется в `GameplayState` после `GameFieldSystem`.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Core/Systems/PlayerSpawnSystem.cs` | Create | IGameSystem: первый спавн и ре-спавн игрока |
| `CB-client/Assets/Scripts/Core/GameContext.cs` | Modify | Добавить PlayerView Player { get; set; } |
| `CB-client/Assets/Scripts/States/GameplayState.cs` | Modify | Зарегистрировать PlayerSpawnSystem |

- [x] [Task 1: PlayerSpawnSystem, GameContext and GameplayState](tasks/task-1.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
