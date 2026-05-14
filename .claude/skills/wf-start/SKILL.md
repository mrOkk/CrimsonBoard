---
name: wf-start
description: "Start a new task or resume in-progress work. Single entry point for the workflow.
  With TASK-NUMBER: reads docs/tasks/, creates branch + worklog or resumes the existing one.
  Without args: lists active worklogs to choose from."
allowed-tools:
  - Read
  - Write
  - Edit
  - Glob
  - Grep
  - Bash
  - Skill
---

## Purpose

Entry router. Reads task description from `docs/tasks/`, creates branch + worklog for new tasks, resumes from last known phase for existing ones.

## Task file format

Task files live at `docs/tasks/{task-number}-{short-name}.md`.  
Example: `docs/tasks/42-add-login-page.md`

The filename itself provides both the task number and the branch slug.

## Flow

### New task — `/wf-start <task-number>`

1. **Find the task file:**
   Glob `docs/tasks/<task-number>-*.md`. If not found, show error:
   ```
   Task file docs/tasks/<task-number>-*.md not found.
   Create it first using the template at .claude/templates/task.md
   ```

2. **Extract info from filename:**
   - `TASK_NUMBER` = `<task-number>` (e.g. `42`)
   - `SHORT_NAME` = the `{short-name}` part of the filename (e.g. `add-login-page`)
   - `BRANCH_NAME` = `feature/<task-number>-<short-name>`

3. **Read task file.** Show the `## Description` section to confirm scope with user.

4. **Check working tree is clean:**
   ```
   python tools/git_branch.py clean
   ```
   On `GIT_BRANCH FAIL: dirty_tree` — show unstaged files and ask user to confirm before continuing.

5. **Create branch:**
   ```
   git checkout -b feature/<task-number>-<short-name>
   ```

6. **Scaffold worklog.** Read `.claude/templates/worklog.md`, substitute placeholders:
   - `{{TICKET_ID}}` → `<task-number>`
   - `{{TICKET_TITLE}}` → `<short-name>` (hyphens → spaces, title-cased)
   - `{{TASK_FILE}}` → `docs/tasks/<task-number>-<short-name>.md`
   - `{{BRANCH_NAME}}` → `feature/<task-number>-<short-name>`
   - `{{USER_KEY}}` → output of `git config user.name` (lowercased, spaces → hyphens)
   - `{{ISO_TIMESTAMP}}` → current UTC time

   Determine `<user>` = same as USER_KEY. Write to:
   ```
   docs/worklogs/<user>/<task-number>-<short-name>/worklog.md
   ```
   Create `tasks/` subdirectory in the same folder.

7. **Dispatch Phase 1:**
   ```
   Skill: wf-discovery
   Args: worklog=docs/worklogs/<user>/<task-number>-<short-name>/worklog.md
   ```

### Resume — `/wf-start <task-number>` (worklog already exists)

1. Glob `docs/worklogs/*/<task-number>-*/worklog.md` — find the worklog.
2. Read metadata: `current_phase`, `status`, `branch`.
3. If `status == done` — inform user and stop.
4. Checkout the worklog branch:
   ```
   git checkout <branch>
   ```
5. Dispatch to the phase matching `current_phase`:
   - `1` → `wf-discovery`
   - `2` → `wf-plan`
   - `3` → `wf-implement`

### No-arg — `/wf-start`

1. Glob `docs/worklogs/*/*/worklog.md`.
2. Show table of active worklogs (status == active), sorted by `updated_at` desc.
3. Ask user to pick one by number.
4. Checkout its branch and dispatch to `current_phase` as in Resume.

## Routing table

| Invocation | Worklog? | Task file? | → |
|---|---|---|---|
| `/wf-start <N>` | No | Yes | New task flow |
| `/wf-start <N>` | Yes | — | Resume |
| `/wf-start <N>` | No | No | Error: create task file first |
| `/wf-start` | — | — | List + pick |
