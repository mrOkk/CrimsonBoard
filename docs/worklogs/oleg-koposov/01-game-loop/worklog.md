# 01 Game Loop

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `01` |
| `task_file` | `docs/tasks/01-game-loop.md` |
| `branch` | `feature/01-game-loop` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-19T09:39:59Z` |
| `updated_at` | `2026-05-19T09:45:02Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-19T09:45:02Z | 1 → 2 | Discovery signed off |
| 2026-05-19T09:39:59Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach
Bootstrap MonoBehaviour (`EntryPoint`) on the root GameObject of a single persistent scene owns a `GameStateMachine` and a `GameContext` singleton. The FSM uses an `IGameState` interface (`Enter / Exit / Tick`) with five concrete states. `GameContext` is a plain C# singleton that stores all shared dependencies (configs, UI refs, world). Core gameplay state drives a `GameplaySystemRunner` — a list of `IGameSystem` objects ticked every frame (ECS-like).

### State Transitions
```
[InitState] → [TapToStartState] → [GameplayState] ⇄ [PauseState]
                                          ↓
                                   [GameOverState] → [TapToStartState] (restart)
PauseState accessible from any post-init state via GameStateMachine.RequestPause()
```

### Scope
**In:**
- `EntryPoint.cs` MonoBehaviour (scene root)
- `GameContext.cs` singleton
- `GameStateMachine.cs` + `IGameState.cs`
- Five state stubs: `InitState`, `TapToStartState`, `GameplayState`, `PauseState`, `GameOverState`
- `GameplaySystemRunner.cs` + `IGameSystem.cs` (empty runner, no systems yet)
- Assembly definition (`GameLoop.asmdef`)
- Folder structure under `CB-client/Assets/Scripts/`

**Out:**
- Actual gameplay systems (future tasks)
- UI implementation / prefabs
- Asset loading / addressables
- Config file content
- Specific game rules

### Key Constraints
- Unity 2022.3 LTS / URP — no DOTS, no Visual Scripting
- No DI framework — `GameContext` singleton wires deps manually
- `.meta` files must be committed alongside every new `.cs` / folder

### Files to Touch
- `CB-client/Assets/Scripts/` — all new (empty today)
- `CB-client/Assets/Scenes/SampleScene.unity` — rename / configure EntryPoint GameObject
- `CB-client/Assets/Scripts/GameLoop.asmdef` (new)

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
