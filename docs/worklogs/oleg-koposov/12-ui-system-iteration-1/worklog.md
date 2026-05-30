# 12 Ui System Iteration 1

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `12` |
| `task_file` | `docs/tasks/12-ui-system-iteration-1.md` |
| `branch` | `feature/12-ui-system-iteration-1` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-30T16:05:43Z` |
| `updated_at` | `2026-05-30T16:09:45Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-30T16:09:45Z | 1 → 2 | Discovery signed off |
| 2026-05-30T16:05:43Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach
Единый фасад `UiRoot` (MonoBehaviour) на одном UI-префабе.  
View-объекты добавляются как дочерние GameObject-ы префаба; `UiRoot` обнаруживает их через `GetComponentsInChildren<BaseView>(true)` в `Awake` и регистрирует в `Dictionary<Type, BaseView>`.

### Scope

**In:**
- `BaseView` — abstract MonoBehaviour c `virtual Show()`, `Hide()`, `Tick(float)`
- `UiRoot` — MonoBehaviour-фасад: авто-регистрация View по типу, `Show<T>()`, `Hide<T>()`, `GetView<T>()`, `Tick(float)`
- `GameContext.UiRoot` — ссылка на фасад для доступа из состояний
- `EntryPoint` — `[SerializeField] UiRoot _uiRoot`, присваивает `context.UiRoot`, вызывает `_uiRoot?.Tick()` после `_fsm.Tick()`
- UI-префаб — создаётся вручную в Unity Editor (out of code scope)

**Out:**
- Конкретные View (TapToStartView, GameplayView, GameOverView)
- Наполнение UI-префаба (GameObject-ы, Canvas, UI-компоненты)
- Привязка состояний к конкретным View
- Анимации показа/скрытия

### Key Constraints
- Все View должны быть дочерними объектами префаба `UiRoot` (для авто-обнаружения)
- TextMeshPro для текста (проектная конвенция)
- Сборка: `CB-client.asmdef`, namespace `CrimsonBoard`

### Files to Touch
- NEW `CB-client/Assets/Scripts/UI/BaseView.cs`
- NEW `CB-client/Assets/Scripts/UI/UiRoot.cs`
- MOD `CB-client/Assets/Scripts/Core/GameContext.cs`
- MOD `CB-client/Assets/Scripts/Core/EntryPoint.cs`

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
