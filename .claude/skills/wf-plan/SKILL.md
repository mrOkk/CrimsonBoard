---
name: wf-plan
description: "Phase 2 of wf workflow. Builds a step-by-step implementation plan from Discovery.
  Writes cross-cutting headers to worklog ## Tasks and per-Task files. Transitions to Phase 3 (wf-implement)."
user-invocable: false
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

Phase 2 driver. Turn the agreed Discovery into a self-contained plan split between worklog and per-Task files.

## Precondition

Read worklog — verify `## Discovery` is non-empty and contains a scope decision before proceeding.

## Flow

1. **Read `## Discovery`** in worklog. The chosen approach and constraints are the source of truth.
   Also re-read `## Comments` in the task file (`task_file` metadata field) for any late-breaking notes.

2. **List all files** to create or modify with one-line purpose each.

3. **Write cross-cutting headers** to worklog `## Tasks`:
   ```markdown
   **Goal:** <one-paragraph>

   **Architecture:** <2-3 sentences mirroring Discovery>

   **File structure:**
   | Path | Type | Purpose |
   |---|---|---|
   | <path> | Create/Modify/Test | <one line> |
   ```

4. **For each Task, create `tasks/task-<N>.md`** inside the worklog folder:
   ```markdown
   # Task <N>: <name>

   ## Plan

   **Files:**
   - Create: <path>
   - Modify: <path>

   **Commit message:** <task-number> <imperative description>

   ### Steps
   1. <2–5 min step with exact code or precise intent + local precedent citation>
   2. ...

   ## Implementation
   <!-- Filled in Phase 3 -->
   ```

5. **Append checklist** to worklog `## Tasks`:
   ```markdown
   - [ ] [Task <N>: <name>](tasks/task-<N>.md)
   ```

6. **Self-review.** Check:
   - Every Discovery item has a Task covering it
   - No placeholders (TBD, "implement later", "similar to Task N")
   - Steps are 2–5 min granularity

7. **Policy gate.** Show task list. Ask:
   ```
   Proceed to Implement? [y / revise / stop]
   ```
   - `y` → boundary commit + dispatch Phase 3
   - `revise` → revise plan, re-ask
   - `stop` → stop (stays at Phase 2 for next session)

8. **Boundary commit:**
   ```
   python tools/worklog.py advance <worklog_path> --from 2 --to 3 --note "Plan signed off"
   python tools/git_branch.py commit-push \
     --add docs/worklogs/<user>/<slug>/worklog.md \
     --add docs/worklogs/<user>/<slug>/tasks/ \
     --message "<task-number> Sign off Phase 2 (Plan)"
   ```

9. **Dispatch:**
   ```
   Skill: wf-implement
   Args: worklog=<worklog_path>
   ```
