# 13 Ui Views Implementation

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `13` |
| `task_file` | `docs/tasks/13-ui-views-implementation.md` |
| `branch` | `feature/13-ui-views-implementation` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-30T18:34:36Z` |
| `updated_at` | `2026-05-30T18:42:04Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-30T18:42:04Z | 1 → 2 | Discovery signed off |
| 2026-05-30T18:34:36Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach
4 конкретных View-класса + `GameStats`; данные пушатся через события либо поллятся в `Tick()` из `GameContext.Instance`. Состояния (TapToStartState, GameplayState, PauseState, GameOverState) показывают/скрывают свой View в `Enter()`/`Exit()` через `_context.UiRoot`. Аудио-настройки идут напрямую в `AudioListener`. Рестарт с PostBattle минует PreBattle через флаг `autoStart` в `TapToStartState`.

### Scope

**In:**
- `GameStats` — score (int) + elapsed battle time (float), Reset/AddScore/Tick
- `GameContext.Stats` — ссылка на GameStats
- `PreBattleView` — заставка «tap to start», обнаруживает любой ввод (клавиша/тап) в Tick, отправляет событие в TapToStartState
- `HudView` — здоровье, оружие, «∞» патронов, время, счёт, кнопка меню; поллинг GameContext.Instance в Tick
- `MenuView` — Continue/Restart, аудио-переключатель и слайдер (AudioListener); события пробрасываются в PauseState
- `PostBattleView` — итоговый счёт/время, кнопка рестарта; событие → GameOverState
- Изменения в TapToStartState (флаг autoStart + Show/Hide PreBattleView)
- Изменения в GameplayState (Show/Hide HudView + AddScore на убийство врага)
- Изменения в PauseState (Show/Hide MenuView + обработка Continue/Restart)
- Изменения в GameOverState (Show/Hide PostBattleView + обработка рестарта)

**Out:**
- Полноценный AudioManager / AudioMixer
- Анимации переходов между экранами
- Локализация текста
- Система оружия с реальными патронами
- UI-префаб и сцена (создаётся вручную в Unity Editor)

### Key Constraints
- TextMeshPro для всего текста (проектная конвенция)
- `GameContext.Instance` используется из View-Tick (существующий паттерн статического синглтона)
- namespace `CrimsonBoard`, сборка `CB-client.asmdef`
- Бесконечные патроны — всегда показываем «∞»
- Рестарт с PostBattle: `TapToStartState(autoStart: true)` инициализирует мир и сразу переходит в `GameplayState`

### Files to Touch
- NEW `CB-client/Assets/Scripts/Core/GameStats.cs`
- NEW `CB-client/Assets/Scripts/UI/Views/PreBattleView.cs`
- NEW `CB-client/Assets/Scripts/UI/Views/HudView.cs`
- NEW `CB-client/Assets/Scripts/UI/Views/MenuView.cs`
- NEW `CB-client/Assets/Scripts/UI/Views/PostBattleView.cs`
- MOD `CB-client/Assets/Scripts/Core/GameContext.cs`
- MOD `CB-client/Assets/Scripts/States/TapToStartState.cs`
- MOD `CB-client/Assets/Scripts/States/GameplayState.cs`
- MOD `CB-client/Assets/Scripts/States/PauseState.cs`
- MOD `CB-client/Assets/Scripts/States/GameOverState.cs`

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
