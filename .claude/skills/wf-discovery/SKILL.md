---
name: wf-discovery
description: "Phase 1 of wf workflow. Reads task description from docs/tasks/, explores
  the codebase, asks clarifying questions, proposes an approach, writes findings to
  ## Discovery in the worklog. Transitions to Phase 2 (wf-plan)."
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

Phase 1 driver. Understand the task from `docs/tasks/`, agree on approach, write `## Discovery` in worklog.

## Flow

1. **Read worklog.** Extract `ticket_id`, `task_file` from metadata.

2. **Read task file** at `task_file` path. Parse:
   - `## Description` — the authoritative statement of what to build
   - `## Comments` — existing decisions, constraints, or notes from user/LLM to carry forward

3. **Explore codebase.** Read relevant files (use Grep/Glob). Read architecture docs in `docs/architecture/` if they exist.

4. **Clarify scope** (one question at a time). Cover:
   - What exactly should be changed / added
   - What is explicitly out of scope
   - Key constraints (performance, backward compat, other systems affected)
   Limit to ~5 questions. Multiple-choice when possible. Skip questions already answered in `## Comments`.

5. **Propose 1–3 approaches** with tradeoffs. Lead with your recommendation.

6. **Write `## Discovery`** to worklog. Include:
   - Chosen approach + rationale
   - Scope boundary (what is in / out)
   - Key constraints noted
   - Files likely to be touched (coarse list)

7. **Append LLM comment to task file.** Add a comment to `## Comments` in the task file summarising the agreed approach:
   ```markdown
   **[Copilot, <date>]:** Discovery complete. Approach: <one sentence>. Scope: <in/out>.
   ```

8. **Policy gate.** Show 1-sentence summary of Discovery. Ask:
   ```
   Proceed to Plan? [y / revise / stop]
   ```
   - `y` → boundary commit + dispatch Phase 2
   - `revise` → revise Discovery, re-ask
   - `stop` → set status done, stop

9. **Boundary commit:**
   ```
   python tools/worklog.py advance <worklog_path> --from 1 --to 2 --note "Discovery signed off"
   python tools/git_branch.py commit-push \
     --add docs/worklogs/<user>/<slug>/worklog.md \
     --add docs/tasks/<task_file> \
     --message "<task-number> Sign off Phase 1 (Discovery)"
   ```

10. **Dispatch:**
    ```
    Skill: wf-plan
    Args: worklog=<worklog_path>
    ```
