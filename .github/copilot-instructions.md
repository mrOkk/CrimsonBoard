# CrimsonBoard – Copilot Instructions

## AI Workflow

This project uses a 3-phase AI workflow for all tasks.

**Entry point:** `/wf-start <task-number>` to start or resume, `/wf-start` to list active work.

Tasks are defined as individual files in `docs/tasks/` with the naming convention  
`{task-number}-{short-name}.md` (e.g. `docs/tasks/42-add-login-page.md`).  
Each file contains a `## Description` and a `## Comments` block for user/LLM notes.  
Create a task file from `.claude/templates/task.md` before running `/wf-start`.

**Phases:**
1. **Discovery** — read task file, explore codebase, agree on approach
2. **Plan** — break into self-contained per-Task files with step-by-step instructions
3. **Implement** — execute Tasks one by one, commit each

State lives in `docs/worklogs/<user>/<task-number>-<short-name>/worklog.md`.  
Helper scripts: `tools/worklog.py` (phase transitions), `tools/git_branch.py` (branch/commit).

> **Requires** `.vscode/settings.json`: `"github.copilot.chat.skillTool.enabled": true`

---

## Project Overview

CrimsonBoard is a game project with a Unity client (`CB-client/`). The repo name implies a server component may be added alongside the client in the future.

## Repository Structure

```
CrimsonBoard/
└── CB-client/          # Unity 2022.3 LTS project
    ├── Assets/         # Game assets and C# scripts
    ├── Packages/       # Unity Package Manager manifest
    └── ProjectSettings/
```

All game code lives under `CB-client/Assets/`. Open the project in Unity by pointing the Unity Hub to the `CB-client/` folder.

## Unity Version & Rendering

- **Unity 2022.3.62f3 (LTS)**
- **Universal Render Pipeline (URP) 14.0.12** — use URP-compatible shaders and materials only; built-in pipeline shaders won't work
- URP quality tiers are configured in `Assets/Settings/`: `URP-Performant`, `URP-Balanced`, `URP-HighFidelity`

## Meta Files

Every asset in `Assets/` has a paired `.meta` file that Unity uses for GUIDs. **Always commit `.meta` files alongside their assets.** Deleting or regenerating a `.meta` file breaks all scene/prefab references to that asset.

## Key Conventions

- **Assembly Definitions (`.asmdef`)**: use them to partition code into assemblies to keep compile times short and enforce dependency boundaries.
- **TextMeshPro** is the standard for all in-game text — do not use the legacy `UnityEngine.UI.Text` component.
- **URP only**: never add `Standard` shader materials; use `Universal Render Pipeline/Lit` or custom URP shader graphs.
- Scenes go in `Assets/Scenes/`. Scene profiles (post-processing) are in `Assets/Settings/`.
