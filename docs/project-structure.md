# CrimsonBoard – Project Structure

## Overview

CrimsonBoard is a game project with a Unity client (`CB-client/`). The repo name implies a server component may be added alongside the client in the future.

## Repository Structure

```
CrimsonBoard/
├── CB-client/          # Unity 2022.3 LTS project
│   ├── Assets/         # Game assets and C# scripts
│   ├── Packages/       # Unity Package Manager manifest
│   └── ProjectSettings/
├── docs/
│   ├── tasks/          # Task definitions: {task-number}-{short-name}.md
│   └── worklogs/       # AI workflow state per user
└── tools/              # Helper scripts (worklog.py, git_branch.py)
```

All game code lives under `CB-client/Assets/`. Open the project in Unity by pointing the Unity Hub to the `CB-client/` folder.

## Unity Version & Rendering

- **Unity 2022.3.62f3 (LTS)**
- **Universal Render Pipeline (URP) 14.0.12** — use URP-compatible shaders and materials only; built-in pipeline shaders won't work
- URP quality tiers are configured in `Assets/Settings/`: `URP-Performant`, `URP-Balanced`, `URP-HighFidelity`

## Key Packages

| Package | Version |
|---|---|
| Universal RP | 14.0.12 |
| TextMeshPro | 3.0.7 |
| Timeline | 1.7.7 |
| Unity Test Framework | 1.1.33 |
| Visual Scripting | 1.9.4 |

## Testing

Tests are run via the **Unity Editor**: *Window → General → Test Runner*.

- **Edit Mode tests**: placed in an `Editor/` subfolder inside `Assets/`, with an `asmdef` referencing `UnityEditor.TestRunner`.
- **Play Mode tests**: placed anywhere in `Assets/` with an `asmdef` referencing `UnityEngine.TestRunner`.

## Meta Files

Every asset in `Assets/` has a paired `.meta` file that Unity uses for GUIDs. **Always commit `.meta` files alongside their assets.** Deleting or regenerating a `.meta` file breaks all scene/prefab references to that asset.

## Key Conventions

- **Assembly Definitions (`.asmdef`)**: use them to partition code into assemblies to keep compile times short and enforce dependency boundaries.
- **TextMeshPro** is the standard for all in-game text — do not use the legacy `UnityEngine.UI.Text` component.
- **URP only**: never add `Standard` shader materials; use `Universal Render Pipeline/Lit` or custom URP shader graphs.
- Scenes go in `Assets/Scenes/`. Scene profiles (post-processing) are in `Assets/Settings/`.
