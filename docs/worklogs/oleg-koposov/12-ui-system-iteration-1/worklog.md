# 12 Ui System Iteration 1

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `12` |
| `task_file` | `docs/tasks/12-ui-system-iteration-1.md` |
| `branch` | `feature/12-ui-system-iteration-1` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-30T16:05:43Z` |
| `updated_at` | `2026-05-30T16:25:33Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-30T16:25:33Z | 2 → 3 | Plan signed off |
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

**Goal:** Реализовать базовую UI-инфраструктуру — абстрактный `BaseView` и фасад `UiRoot` — и подключить её к игровому циклу через `GameContext` и `EntryPoint`. После этого любое состояние (FSM) сможет управлять видимостью View через `context.UiRoot.Show<T>()`.

**Architecture:** `UiRoot` — MonoBehaviour, который в `Awake` сканирует дочерние объекты через `GetComponentsInChildren<BaseView>(true)` и регистрирует каждый View в `Dictionary<Type, BaseView>`. `EntryPoint` держит `[SerializeField] UiRoot _uiRoot`, передаёт его в `GameContext.UiRoot` при `Awake` и вызывает `_uiRoot?.Tick(deltaTime)` после `_fsm.Tick(deltaTime)` в `Update`.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/UI/BaseView.cs` | Create | Абстрактный MonoBehaviour с Show/Hide/Tick |
| `CB-client/Assets/Scripts/UI/UiRoot.cs` | Create | MonoBehaviour-фасад: регистрация и управление View |
| `CB-client/Assets/Scripts/Core/GameContext.cs` | Modify | Добавить свойство `UiRoot UiRoot` |
| `CB-client/Assets/Scripts/Core/EntryPoint.cs` | Modify | Добавить `_uiRoot`, присвоить `context.UiRoot`, вызвать Tick |

- [x] [Task 1: Create UI infrastructure (BaseView + UiRoot)](tasks/task-1.md)
- [x] [Task 2: Integrate UiRoot into GameContext and EntryPoint](tasks/task-2.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
