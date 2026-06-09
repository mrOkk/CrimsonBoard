# CrimsonBoard AGENTS.md

Project: **Crimson Gambit** (repo uses legacy name "CrimsonBoard")

## Architecture

- **Unity 2022.3.62f3 LTS** with **URP 14.0.12** — use URP shaders only, never built-in pipeline
- Entry point: `CB-client/Assets/Scripts/Core/EntryPoint.cs`
- ECS-like pattern: `GameContext` singleton + `IGameSystem` interface run by `GameplaySystemRunner`
- State machine: `GameStateMachine` with states (Init, TapToStart, Gameplay, Pause, GameEnd)
- Assembly definitions required — see `Scripts/CB-client.asmdef` and sub-assemblies

### GameContext Guidelines

- **`GameContext` must not contain systems** — it should only hold data entities and shared state
- If other code needs data from a system, extract that data into a separate entity class
- Example: Replace `GameBoardSystem` references with a `GameBoard` class that holds board data
- Systems should read from and write to these data entities, not expose their internal state directly
- Active opponents list should also be stored in `GameBoard` or similar data entity, not in a system
- **Systems may only be dependencies of other systems** — passed via constructor, not stored in GameContext
- Data entities, states, and UI must not hold references to systems

## Code Organization

```
CB-client/Assets/Scripts/
├── Core/        # EntryPoint, GameContext, GameStateMachine, systems, pools, configs
├── Entities/    # MonoBehaviour components
├── Gameplay/    # Game-specific logic
├── States/      # Game state implementations
├── UI/          # UI views and controllers
├── Tests/       # Play Mode tests
└── Editor/      # Edit Mode tests, editor tools
```

## Unity Conventions

- **Always commit `.meta` files** with their assets — deleting/regenerating breaks GUIDs
- **TextMeshPro only** — never use legacy `UnityEngine.UI.Text`
- **URP shaders only** — use `Universal Render Pipeline/Lit` or URP shader graphs
- **UniTask only** — never use coroutines, use UniTask for async operations
- Scenes: `Assets/Scenes/`
- URP quality tiers: `Assets/Settings/URP-*`

## Testing

Run via Unity Editor: *Window → General → Test Runner*

- **Edit Mode**: place in `Editor/` subfolder with asmdef referencing `UnityEditor.TestRunner`
- **Play Mode**: place anywhere with asmdef referencing `UnityEngine.TestRunner`

## AI Workflow (3-Phase)

**Entry point:** Use `wf-start` skill

1. **Discovery** — read task, explore codebase, agree on approach
2. **Plan** — break into step-by-step tasks
3. **Implement** — execute tasks, commit each

**Task files:** `docs/tasks/{number}-{name}.md` (template: `.claude/templates/task.md`)
**Worklogs:** `docs/worklogs/{user}/{number}-{name}/worklog.md`
**Tools:** `tools/worklog.py`, `tools/git_branch.py`

## Git

- **Do not** add `Co-authored-by:` trailers to commits

## Code Style

See `CodeStyle.md` for formatting conventions.
