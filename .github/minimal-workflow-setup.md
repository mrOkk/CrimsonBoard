# Minimal AI Workflow Setup (Discovery → Plan → Implement)

Инструкция для Copilot: создать 3-фазный workflow в новом репозитории.
Фазы: **Discovery** (1), **Plan** (2), **Implement** (3). Создание ветки — часть входного скилла.

---

## Что нужно создать

```
.claude/
  skills/
    wf-start/SKILL.md          # входной роутер: создаёт ветку + worklog, запускает Phase 1
    wf-discovery/SKILL.md      # Phase 1: разбор задачи, выбор подхода
    wf-plan/SKILL.md           # Phase 2: пошаговый план реализации
    wf-implement/SKILL.md      # Phase 3: исполнение плана
  templates/
    worklog.md                 # шаблон файла состояния тикета
tools/
  worklog.py                   # read/write состояния worklog
  git_branch.py                # создание ветки, commit-push, проверка чистоты дерева
.github/
  copilot-instructions.md      # краткий контекст: как устроен workflow
.vscode/
  settings.json                # включить skill tool
docs/
  worklogs/<user>/             # сюда пишутся worklog-файлы
```

---

## 1. Шаблон worklog (`.claude/templates/worklog.md`)

Это файл состояния тикета. `current_phase` и `status` — единственные поля, которые читает роутер при возобновлении работы.

```markdown
# {{TICKET_ID}} {{TICKET_TITLE}}

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `{{TICKET_ID}}` |
| `branch` | `{{BRANCH_NAME}}` |
| `user_key` | `{{USER_KEY}}` |
| `current_phase` | `1` |
| `status` | `active` |
| `created_at` | `{{ISO_TIMESTAMP}}` |
| `updated_at` | `{{ISO_TIMESTAMP}}` |

## Phase History

| When | From → To | Note |
|---|---|---|
| {{ISO_TIMESTAMP}} | — → 1 | Created by wf-start |

## Discovery

<!-- Filled by wf-discovery: scope, chosen approach, design overview. -->

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
```

---

## 2. Скрипты (`tools/`)

### `tools/worklog.py`

Минимальный CLI-обёртка для чтения и обновления метаданных worklog.

```python
#!/usr/bin/env python3
"""Worklog metadata helper.

Subcommands:
    read <path> --field <name>        Print one metadata field value.
    advance <path> --from N --to M --note "<text>"
                                      Validate current_phase==N, set to M,
                                      refresh updated_at, append Phase History row.
    set-status <path> --status (active|done) --note "<text>"
                                      Set status field, append Phase History row.

Exit codes: 0 success, 1 failure.
Stderr on failure: WORKLOG FAIL: <code>\\n<message>
"""
import argparse, re, sys
from datetime import datetime, timezone
from pathlib import Path

def now_iso():
    return datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

def fail(code, msg):
    print(f"WORKLOG FAIL: {code}", file=sys.stderr)
    print(msg, file=sys.stderr)
    sys.exit(1)

def parse_metadata(text):
    """Return dict of metadata fields from the first Markdown table in text."""
    fields = {}
    for line in text.splitlines():
        m = re.match(r'\|\s*`([^`]+)`\s*\|\s*`?([^`|]+?)`?\s*\|', line)
        if m:
            fields[m.group(1).strip()] = m.group(2).strip()
    return fields

def set_field(text, field, value):
    """Replace a metadata field value in the table."""
    # Matches: | `field` | <anything> |
    pattern = re.compile(
        r'(\|\s*`' + re.escape(field) + r'`\s*\|\s*)([^|\n]*)(\s*\|)',
        re.MULTILINE,
    )
    replacement = r'\g<1>`' + value + r'`\g<3>'
    new_text, count = pattern.subn(replacement, text)
    if count == 0:
        fail("unknown_field", f"Field '{field}' not found in metadata table.")
    return new_text

def append_history_row(text, when, direction, note):
    marker = "## Phase History"
    table_header = "| When | From → To | Note |"
    row = f"| {when} | {direction} | {note} |"
    if marker not in text:
        fail("metadata_malformed", "Phase History section not found.")
    # Insert after the table header + separator line
    idx = text.index(table_header)
    end = text.index("\n", idx)
    sep_end = text.index("\n", end + 1)
    return text[: sep_end + 1] + row + "\n" + text[sep_end + 1 :]

def main():
    p = argparse.ArgumentParser()
    sub = p.add_subparsers(dest="cmd")

    r = sub.add_parser("read")
    r.add_argument("path"); r.add_argument("--field", required=True)

    a = sub.add_parser("advance")
    a.add_argument("path"); a.add_argument("--from", dest="from_phase", type=int, required=True)
    a.add_argument("--to", dest="to_phase", type=int, required=True); a.add_argument("--note", required=True)

    s = sub.add_parser("set-status")
    s.add_argument("path"); s.add_argument("--status", required=True); s.add_argument("--note", required=True)

    args = p.parse_args()
    if not args.cmd:
        p.print_help(); sys.exit(1)

    path = Path(args.path)
    if not path.exists():
        fail("worklog_missing", f"File not found: {path}")
    text = path.read_text(encoding="utf-8")
    meta = parse_metadata(text)

    if args.cmd == "read":
        v = meta.get(args.field)
        if v is None:
            fail("unknown_field", f"Field '{args.field}' not found.")
        print(v)

    elif args.cmd == "advance":
        current = int(meta.get("current_phase", 0))
        if current != args.from_phase:
            fail("phase_mismatch", f"current_phase is {current}, expected {args.from_phase}.")
        ts = now_iso()
        text = set_field(text, "current_phase", str(args.to_phase))
        text = set_field(text, "updated_at", ts)
        text = append_history_row(text, ts, f"{args.from_phase} → {args.to_phase}", args.note)
        path.write_text(text, encoding="utf-8")

    elif args.cmd == "set-status":
        prev_status = meta.get("status", "active")
        ts = now_iso()
        text = set_field(text, "status", args.status)
        text = set_field(text, "updated_at", ts)
        text = append_history_row(text, ts, f"{prev_status} → {args.status}", args.note)
        path.write_text(text, encoding="utf-8")

if __name__ == "__main__":
    main()
```

### `tools/git_branch.py`

```python
#!/usr/bin/env python3
"""Git branch helper.

Subcommands:
    current                   Print current branch name.
    clean                     Exit 0 if clean tree, else exit 1 with GIT_BRANCH FAIL: dirty_tree.
    commit-push --add <path> [--add <path>...] --message <text>
                              Stage paths, commit (skip if nothing staged), push.
    compute-slug <text>       Normalize text to branch-safe kebab slug (max 40 chars).

Exit codes: 0 success, 1 failure.
Stderr on failure: GIT_BRANCH FAIL: <code>\\n<message>
"""
import argparse, re, subprocess, sys

def fail(code, msg):
    print(f"GIT_BRANCH FAIL: {code}", file=sys.stderr)
    print(msg, file=sys.stderr)
    sys.exit(1)

def run(cmd, **kw):
    return subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", **kw)

def main():
    p = argparse.ArgumentParser()
    sub = p.add_subparsers(dest="cmd")
    sub.add_parser("current")
    sub.add_parser("clean")
    cp = sub.add_parser("commit-push")
    cp.add_argument("--add", dest="paths", action="append", required=True)
    cp.add_argument("--message", required=True)
    cs = sub.add_parser("compute-slug")
    cs.add_argument("text")

    args = p.parse_args()

    if args.cmd == "current":
        r = run(["git", "rev-parse", "--abbrev-ref", "HEAD"])
        if r.returncode != 0:
            fail("not_a_repo", r.stderr.strip())
        b = r.stdout.strip()
        if b == "HEAD":
            fail("detached_head", "HEAD is detached.")
        print(b)

    elif args.cmd == "clean":
        r = run(["git", "status", "--short"])
        if r.returncode != 0:
            fail("not_a_repo", r.stderr.strip())
        if r.stdout.strip():
            fail("dirty_tree", r.stdout.strip())

    elif args.cmd == "commit-push":
        # Stage
        r = run(["git", "add"] + args.paths)
        if r.returncode != 0:
            fail("git_add_failed", r.stderr.strip())
        # Check if anything is staged
        r = run(["git", "diff", "--cached", "--quiet"])
        if r.returncode != 0:
            # Something staged — commit
            r = run(["git", "commit", "-m", args.message])
            if r.returncode != 0:
                if "hook" in r.stderr.lower():
                    fail("hook_failure", r.stderr.strip())
                fail("commit_failed", r.stderr.strip())
        # Push
        r = run(["git", "push"])
        if r.returncode != 0:
            # Try setting upstream on first push
            branch_r = run(["git", "rev-parse", "--abbrev-ref", "HEAD"])
            branch = branch_r.stdout.strip()
            r2 = run(["git", "push", "--set-upstream", "origin", branch])
            if r2.returncode != 0:
                fail("push_failed", r2.stderr.strip())

    elif args.cmd == "compute-slug":
        slug = re.sub(r"[^a-z0-9]+", "-", args.text.lower()).strip("-")[:40]
        if not slug:
            fail("slug_empty", "Input has no usable ASCII alphanumeric content.")
        print(slug)

    else:
        p.print_help(); sys.exit(1)

if __name__ == "__main__":
    main()
```

---

## 3. Entry skill (`.claude/skills/wf-start/SKILL.md`)

Это единственная пользовательская точка входа. Создаёт ветку и worklog при первом запуске, роутит по `current_phase` при возобновлении.

```markdown
---
name: wf-start
description: "Start a new task or resume in-progress work. Single entry point for the workflow.
  With TASK-ID: creates branch + worklog or resumes the existing one.
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

Entry router. Creates branch + worklog for new tasks; resumes from last known phase for existing ones.

## Flow

### New task — `/wf-start <TASK-ID> <short description>`

1. Confirm slug from `<short description>`:
   ```
   python tools/git_branch.py compute-slug "<short description>"
   ```
   Show proposed branch name `feature/<TASK-ID>-<slug>`. Ask user to confirm or provide different slug.

2. Check working tree is clean:
   ```
   python tools/git_branch.py clean
   ```
   On `GIT_BRANCH FAIL: dirty_tree` — show unstaged files and ask user to confirm before continuing.

3. Create branch:
   ```
   git checkout -b feature/<TASK-ID>-<slug>
   ```

4. Scaffold worklog. Read `.claude/templates/worklog.md`, substitute placeholders:
   - `{{TICKET_ID}}` → task ID
   - `{{TICKET_TITLE}}` → short description
   - `{{BRANCH_NAME}}` → `feature/<TASK-ID>-<slug>`
   - `{{USER_KEY}}` → output of `git config user.name` (lowercased, spaces → hyphens)
   - `{{ISO_TIMESTAMP}}` → current UTC time

   Determine `<user>` = same as USER_KEY. Write to:
   ```
   docs/worklogs/<user>/<TASK-ID>-<slug>/worklog.md
   ```
   Create `tasks/` and `reviews/` subdirectories in the same folder.

5. Dispatch Phase 1:
   ```
   Skill: wf-discovery
   Args: worklog=docs/worklogs/<user>/<TASK-ID>-<slug>/worklog.md
   ```

### Resume — `/wf-start <TASK-ID>` (worklog already exists)

1. Glob `docs/worklogs/*/<TASK-ID>-*/worklog.md` — find the worklog.
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

| Invocation | Worklog? | → |
|---|---|---|
| `/wf-start <ID> <slug>` | No | New task flow |
| `/wf-start <ID>` | Yes | Resume |
| `/wf-start` | — | List + pick |
```

---

## 4. Discovery skill (`.claude/skills/wf-discovery/SKILL.md`)

```markdown
---
name: wf-discovery
description: "Phase 1 of wf workflow. Explore the task, ask clarifying questions,
  propose an approach, write findings to ## Discovery. Transitions to Phase 2 (wf-plan)."
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

Phase 1 driver. Understand the task, agree on approach, write `## Discovery` in worklog.

## Flow

1. **Read worklog.** Extract `ticket_id`, task title from file.

2. **Explore codebase.** Read relevant files (use Grep/Glob). Read architecture docs in `docs/architecture/` if they exist.

3. **Clarify scope** (one question at a time). Cover:
   - What exactly should be changed / added
   - What is explicitly out of scope
   - Key constraints (performance, backward compat, other systems affected)
   Limit to ~5 questions. Multiple-choice when possible.

4. **Propose 1–3 approaches** with tradeoffs. Lead with your recommendation.

5. **Write `## Discovery`** to worklog. Include:
   - Chosen approach + rationale
   - Scope boundary (what is in / out)
   - Key constraints noted
   - Files likely to be touched (coarse list)

6. **Policy gate.** Show 1-sentence summary of Discovery. Ask:
   ```
   Proceed to Plan? [y / revise / stop]
   ```
   - `y` → boundary commit + dispatch Phase 2
   - `revise` → revise Discovery, re-ask
   - `stop` → set status done, stop

7. **Boundary commit:**
   ```
   python tools/worklog.py advance <path> --from 1 --to 2 --note "Discovery signed off"
   python tools/git_branch.py commit-push \
     --add docs/worklogs/<user>/<slug>/worklog.md \
     --message "<TASK-ID> Sign off Phase 1 (Discovery)"
   ```

8. **Dispatch:**
   ```
   Skill: wf-plan
   Args: worklog=<path>
   ```
```

---

## 5. Plan skill (`.claude/skills/wf-plan/SKILL.md`)

```markdown
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

4. **For each Task, create `tasks/task-<N>.md`:**
   ```markdown
   # Task <N>: <name>

   ## Plan

   **Files:**
   - Create: <path>
   - Modify: <path>

   **Commit message:** <TASK-ID> <imperative description>

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
   python tools/worklog.py advance <path> --from 2 --to 3 --note "Plan signed off"
   python tools/git_branch.py commit-push \
     --add docs/worklogs/<user>/<slug>/worklog.md \
     --add docs/worklogs/<user>/<slug>/tasks/ \
     --message "<TASK-ID> Sign off Phase 2 (Plan)"
   ```

9. **Dispatch:**
   ```
   Skill: wf-implement
   Args: worklog=<path>
   ```
```

---

## 6. Implement skill (`.claude/skills/wf-implement/SKILL.md`)

```markdown
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
   f. Flip checkbox in worklog:
      replace `- [ ] [Task <N>` → `- [x] [Task <N>` in worklog `## Tasks`.
   g. Commit worklog update:
      ```
      python tools/git_branch.py commit-push \
        --add docs/worklogs/<user>/<slug>/worklog.md \
        --add docs/worklogs/<user>/<slug>/tasks/task-<N>.md \
        --message "<TASK-ID> Mark Task <N> done"
      ```

3. **After all Tasks done** — show summary of what was implemented.

4. **Write `## What we did`** in worklog: 3–8 lines describing what changed for a reviewer or QA engineer. No class names or internal field names.

5. **Policy gate.** Ask:
   ```
   Mark worklog as done? [y / no]
   ```
   - `y` → close worklog
   - `no` → leave status active, stop (work can continue next session)

6. **Close worklog:**
   ```
   python tools/worklog.py set-status <path> --status done --note "Implemented by wf-implement"
   python tools/git_branch.py commit-push \
     --add docs/worklogs/<user>/<slug>/worklog.md \
     --message "<TASK-ID> Phase 3 complete"
   ```
```

---

## 7. VS Code settings (`.vscode/settings.json`)

```json
{
  "github.copilot.chat.skillTool.enabled": true
}
```

Это единственная обязательная настройка. Без неё Copilot не читает `SKILL.md` и не умеет диспатчить навыки.

---

## 8. Краткий файл контекста (`.github/copilot-instructions.md`)

```markdown
# Copilot Instructions

This project uses a 3-phase AI workflow for tasks.

Entry point: `/wf-start <TASK-ID> <short description>` to start, `/wf-start` to resume.

Phases:
1. Discovery — understand task, agree on approach
2. Plan — break into self-contained Tasks with step-by-step instructions
3. Implement — execute Tasks one by one, commit each

State lives in `docs/worklogs/<user>/<TASK-ID>-<slug>/worklog.md`.
Helper scripts: `tools/worklog.py` (phase transitions), `tools/git_branch.py` (branch/commit).
```

---

## Чеклист создания

Copilot должен создать файлы в следующем порядке (важно: скрипты первыми, skills их уже вызывают):

- [ ] `tools/worklog.py` — скрипт управления состоянием
- [ ] `tools/git_branch.py` — скрипт git-операций
- [ ] `.claude/templates/worklog.md` — шаблон worklog
- [ ] `.claude/skills/wf-start/SKILL.md` — входной роутер
- [ ] `.claude/skills/wf-discovery/SKILL.md` — Phase 1
- [ ] `.claude/skills/wf-plan/SKILL.md` — Phase 2
- [ ] `.claude/skills/wf-implement/SKILL.md` — Phase 3
- [ ] `.github/copilot-instructions.md` — контекст для Copilot
- [ ] `.vscode/settings.json` — добавить `"github.copilot.chat.skillTool.enabled": true`
- [ ] `docs/worklogs/` — создать пустую папку (можно добавить `.gitkeep`)

---

## Как проверить, что работает

1. В VS Code Copilot Chat введи `/wf-start` — должен предложить список worklogs (пустой при первом запуске) или попросить ID задачи.
2. Введи `/wf-start TASK-1 add login page` — должен создать ветку `feature/TASK-1-add-login-page`, файл `docs/worklogs/<user>/TASK-1-add-login-page/worklog.md` и запустить Discovery.
3. После Discovery ответь `y` — должен создать `tasks/task-1.md` с планом и перейти к Implement.
4. Закрой сессию. Открой снова, введи `/wf-start TASK-1` — должен возобновить с Phase 3.

Если шаг 1 не работает (нет автодополнения `/wf-start`) — проверь `github.copilot.chat.skillTool.enabled: true` в `.vscode/settings.json`.
