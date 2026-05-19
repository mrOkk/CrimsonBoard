# 01 Game Loop

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `01` |
| `task_file` | `docs/tasks/01-game-loop.md` |
| `branch` | `feature/01-game-loop` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-19T09:39:59Z` |
| `updated_at` | `2026-05-19T09:48:35Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-19T09:48:35Z | 2 → 3 | Plan signed off |
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

**Goal:** Implement a complete game-loop skeleton for CrimsonBoard — a single bootstrap scene with an `EntryPoint` MonoBehaviour that creates a `GameContext` singleton and a `GameStateMachine` driving five states (Init, TapToStart, Gameplay, Pause, GameOver). Core gameplay state delegates per-frame work to a `GameplaySystemRunner` containing a list of `IGameSystem` objects, enabling future ECS-like systems to plug in without touching the FSM.

**Architecture:** `EntryPoint` (MonoBehaviour) owns `GameContext` (singleton, manual dep storage) and `GameStateMachine` (plain C# FSM). Each state receives both via constructor. `GameplayState` additionally owns a `GameplaySystemRunner`. Pause is requestable from any post-init state through `GameStateMachine.RequestPause()` / `ResumePreviousState()`.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Core/IGameState.cs` | Create | FSM state interface |
| `CB-client/Assets/Scripts/Core/IGameSystem.cs` | Create | ECS-like system interface |
| `CB-client/Assets/Scripts/Core/GameContext.cs` | Create | Singleton dep container |
| `CB-client/Assets/Scripts/Core/GameStateMachine.cs` | Create | FSM driver |
| `CB-client/Assets/Scripts/States/InitState.cs` | Create | Init state stub |
| `CB-client/Assets/Scripts/States/TapToStartState.cs` | Create | Tap-to-start state stub |
| `CB-client/Assets/Scripts/States/GameplayState.cs` | Create | Core gameplay state stub |
| `CB-client/Assets/Scripts/States/PauseState.cs` | Create | Pause state stub |
| `CB-client/Assets/Scripts/States/GameOverState.cs` | Create | Game-over state stub |
| `CB-client/Assets/Scripts/Gameplay/GameplaySystemRunner.cs` | Create | ECS-like system runner |
| `CB-client/Assets/Scripts/Core/EntryPoint.cs` | Create | Bootstrap MonoBehaviour |
| `*.meta` files for all above + new folders | Create | Unity GUID tracking |

- [ ] [Task 1: Scaffold folders and assembly definition](tasks/task-1.md)
- [ ] [Task 2: Core interfaces — IGameState and IGameSystem](tasks/task-2.md)
- [ ] [Task 3: GameContext singleton](tasks/task-3.md)
- [ ] [Task 4: GameStateMachine](tasks/task-4.md)
- [ ] [Task 5: Five state stubs](tasks/task-5.md)
- [ ] [Task 6: GameplaySystemRunner](tasks/task-6.md)
- [ ] [Task 7: EntryPoint MonoBehaviour](tasks/task-7.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
