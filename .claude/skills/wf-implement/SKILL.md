---
name: wf-implement
description: "Phase 3 of wf workflow. Executes the plan task-by-task, commits each task,
  and marks the worklog done."
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

Phase 3 driver. Execute each Task from `tasks/task-<N>.md` sequentially, commit per task, close the worklog.

## Flow

1. **Read worklog `## Tasks` checklist.** Find all unchecked `- [ ]` entries.

2. **For each unchecked Task** (in order, one at a time):
   a. Read `tasks/task-<N>.md` `## Plan`.
   b. Follow steps exactly. Implement the code changes.
   c. Run verification commands from the plan.
   d. Commit using the Task's commit message:
      ```
      python tools/git_branch.py commit-push \
        --add <task files> \
        --message "<commit message from plan>"
      ```
   e. Write `## Implementation` section in `tasks/task-<N>.md`:
      ```markdown
      ## Implementation
      **Status:** DONE
      **Summary:** <1-2 sentences: what was actually done, deviations from plan if any>
      ```
   f. Flip checkbox in worklog: replace `- [ ] [Task <N>` → `- [x] [Task <N>` in worklog `## Tasks`.
   g. Commit worklog + task file update:
      ```
      python tools/git_branch.py commit-push \
        --add docs/worklogs/<user>/<slug>/worklog.md \
        --add docs/worklogs/<user>/<slug>/tasks/task-<N>.md \
        --message "<task-number> Mark Task <N> done"
      ```

3. **After all Tasks done** — show summary of what was implemented.

4. **Write `## What we did`** in worklog: 3–8 lines describing what changed for a reviewer or QA engineer. No class names or internal field names.

5. **Append LLM comment to task file.** Add to `## Comments` in `docs/tasks/<task_file>`:
   ```markdown
   **[Copilot, <date>]:** Implementation complete. <1-2 sentence summary of what was built>.
   ```

6. **Policy gate.** Ask:
   ```
   Mark worklog as done? [y / no]
   ```
   - `y` → close worklog
   - `no` → leave status active, stop (work can continue next session)

7. **Close worklog:**
   ```
   python tools/worklog.py set-status <worklog_path> --status done --note "Implemented by wf-implement"
   python tools/git_branch.py commit-push \
     --add docs/worklogs/<user>/<slug>/worklog.md \
     --add docs/tasks/<task_file> \
     --message "<task-number> Phase 3 complete"
   ```
