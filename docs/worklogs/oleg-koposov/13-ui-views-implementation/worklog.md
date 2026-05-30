# 13 Ui Views Implementation

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `13` |
| `task_file` | `docs/tasks/13-ui-views-implementation.md` |
| `branch` | `feature/13-ui-views-implementation` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `done`|
| `created_at` | `2026-05-30T18:34:36Z` |
| `updated_at` | `2026-05-30T19:31:48Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-30T19:31:48Z | active → done | Implemented by wf-implement |
| 2026-05-30T18:46:05Z | 2 → 3 | Plan signed off |
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

**Goal:** Реализовать четыре View-экрана (PreBattleView, HudView, MenuView, PostBattleView) и базовый трекер игровой статистики (GameStats). Каждое FSM-состояние показывает свой экран при входе и скрывает при выходе. HudView обновляется через Tick, поллируя GameContext.Instance. MenuView управляет звуком через AudioListener. Рестарт из PostBattleView минует pre-battle через флаг autoStart в TapToStartState.

**Architecture:** View-классы расширяют BaseView и хранят [SerializeField]-ссылки на UI-элементы; данные берут из GameContext.Instance в Tick() либо при Show(). Состояния управляют видимостью через context.UiRoot.Show\<T\>()/Hide\<T\>() и подписываются на события View в Enter()/Exit(). GameStats живёт в GameContext.Stats и тикает из GameplayState.Tick().

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Core/GameStats.cs` | Create | Score + elapsed time, Reset/AddScore/Tick |
| `CB-client/Assets/Scripts/UI/Views/PreBattleView.cs` | Create | «Tap to start» экран, обнаружение ввода |
| `CB-client/Assets/Scripts/UI/Views/HudView.cs` | Create | Боевой HUD: здоровье, оружие, патроны, время, счёт |
| `CB-client/Assets/Scripts/UI/Views/MenuView.cs` | Create | Пауза: Continue/Restart, аудио-настройки |
| `CB-client/Assets/Scripts/UI/Views/PostBattleView.cs` | Create | Итоговый экран: счёт/время, рестарт |
| `CB-client/Assets/Scripts/Core/GameContext.cs` | Modify | Добавить GameStats Stats |
| `CB-client/Assets/Scripts/States/TapToStartState.cs` | Modify | Show/Hide PreBattleView, флаг autoStart |
| `CB-client/Assets/Scripts/States/GameplayState.cs` | Modify | Show/Hide HudView, тик Stats, AddScore на убийство |
| `CB-client/Assets/Scripts/States/PauseState.cs` | Modify | Show/Hide MenuView, Continue/Restart |
| `CB-client/Assets/Scripts/States/GameOverState.cs` | Modify | Show/Hide PostBattleView, рестарт |

- [x] [Task 1: Add GameStats and wire into GameContext](tasks/task-1.md)
- [x] [Task 2: Create PreBattleView and update TapToStartState](tasks/task-2.md)
- [x] [Task 3: Create HudView and update GameplayState](tasks/task-3.md)
- [x] [Task 4: Create MenuView and update PauseState](tasks/task-4.md)
- [x] [Task 5: Create PostBattleView and update GameOverState](tasks/task-5.md)

## What we did

- Добавлен трекер игровой статистики: очки и время в бою накапливаются в ходе матча и сбрасываются при каждом новом старте.
- Реализован экран ожидания перед боем: отображает надпись и ждёт любого нажатия клавиши или касания экрана для перехода в бой.
- Реализован боевой HUD: в реальном времени показывает здоровье игрока, текущее оружие, бесконечные патроны (∞), прошедшее время и счёт; кнопка меню открывает экран паузы.
- Реализован экран паузы (меню): кнопки «Продолжить» и «Рестарт», переключатель и слайдер громкости (работают через Unity AudioListener).
- Реализован итоговый экран: показывает финальный счёт и время, кнопка перезапускает матч напрямую в бой (минуя экран ожидания).
- Все состояния игрового автомата (pre-battle, gameplay, pause, game-over) обновлены: при входе показывают свой экран, при выходе скрывают.
