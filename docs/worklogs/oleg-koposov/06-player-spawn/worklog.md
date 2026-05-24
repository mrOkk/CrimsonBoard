# 06 Player Spawn

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `06` |
| `task_file` | `docs/tasks/06-player-spawn.md` |
| `branch` | `feature/06-player-spawn` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-24T18:01:32Z` |
| `updated_at` | `2026-05-24T18:31:48Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
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

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
