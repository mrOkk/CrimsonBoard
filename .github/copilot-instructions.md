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

See [`docs/project-structure.md`](../docs/project-structure.md) for Unity version, rendering pipeline, packages, conventions, and repo layout.

## Git Conventions

- **Do not** add `Co-authored-by:` trailers to commit messages.
