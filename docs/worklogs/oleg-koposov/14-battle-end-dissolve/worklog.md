# 14 Battle End Dissolve

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `14` |
| `task_file` | `docs/tasks/14-battle-end-dissolve.md` |
| `branch` | `feature/14-battle-end-dissolve` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `done`|
| `created_at` | `2026-06-01T19:55:22Z` |
| `updated_at` | `2026-06-01T20:24:38Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-06-01T20:24:38Z | active → done | Implemented by wf-implement |
| 2026-06-01T20:04:48Z | 2 → 3 | Plan signed off |
| 2026-06-01T20:01:17Z | 1 → 2 | Discovery signed off |
| 2026-06-01T19:55:22Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach

**Подход: DissolveService + прямой рестарт в GameplayState**

Создать статический `DissolveService` с двумя методами:
- `DissolveAndReturn(enemy, occupancyMap, pools, onComplete?)` — полный lifecycle одного врага
- `DissolveAllAndReturn(enemies, occupancyMap, pools, onAllComplete?)` — батч, коллбэк по завершению последнего

`HealthSystem.OnEnemyDeath` делегирует `DissolveService.DissolveAndReturn` вместо inline-колбэка.

При входе в `GameOverState`:
- Вызвать `DissolveService.DissolveAllAndReturn` для всех активных врагов (fire-and-forget)
- Показать PostBattle UI сразу (параллельно с dissolve)

При рестарте: `GameOverState.OnRestart` переходит напрямую в `new GameplayState(context, fsm)` — без `TapToStartState`. `GameplayState.Enter()` уже сбрасывает статистику и здоровье; поле и позиция игрока сохраняются.

Доступ к врагам из `GameOverState`: добавить `EnemySpawnSystem` в `GameContext` (по аналогии с `GameFieldSystem`), устанавливать в конструкторе `GameplayState`.

### Scope

**In:**
- Новый `DissolveService` (статический класс)
- Рефактор `HealthSystem.OnEnemyDeath` (убрать inline dissolve orchestration)
- `GameOverState` — dissolve всех врагов + исправить рестарт
- `GameContext` — добавить `EnemySpawnSystem` property
- `GameplayState` — установить `context.EnemySpawnSystem`

**Out:**
- Изменения в логике волн / спауна
- Изменения в GameField (поле не трогаем)
- Изменения в PlayerSpawnSystem

### Key Constraints

- `DissolveService` — статический, без MonoBehaviour; работает через `DissolveEffect.Play(callback)`
- Список активных врагов копируется при входе в `GameOverState` (snapshot), чтобы не зависеть от мутаций
- При рестарте OccupancyMap для игрока остаётся зарегистрированной — это корректно

### Files Likely Touched

- `CB-client/Assets/Scripts/Core/Systems/DissolveService.cs` (new)
- `CB-client/Assets/Scripts/Core/GameContext.cs`
- `CB-client/Assets/Scripts/Core/Systems/HealthSystem.cs`
- `CB-client/Assets/Scripts/States/GameOverState.cs`
- `CB-client/Assets/Scripts/States/GameplayState.cs`

## Tasks

**Goal:** Доработать завершение битвы: все активные враги уничтожаются через dissolve-эффект при game over; dissolve-lifecycle вынесен в переиспользуемый `DissolveService`; игровое поле и позиция игрока сохраняются при рестарте.

**Architecture:** Статический `DissolveService` инкапсулирует dissolve + pool-return lifecycle для одного врага и для батча. `HealthSystem` делегирует ему; `GameOverState` запускает батч-dissolve параллельно с показом UI. Рестарт переходит напрямую в `GameplayState` (поле и игрок не пересоздаются). Доступ к списку активных врагов обеспечивается через новое свойство `GameContext.EnemySpawnSystem`.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Core/Systems/DissolveService.cs` | Create | Утилита dissolve lifecycle |
| `CB-client/Assets/Scripts/Core/GameContext.cs` | Modify | Добавить `EnemySpawnSystem` property |
| `CB-client/Assets/Scripts/Core/Systems/HealthSystem.cs` | Modify | Делегировать dissolve в `DissolveService` |
| `CB-client/Assets/Scripts/States/GameplayState.cs` | Modify | Установить `context.EnemySpawnSystem` |
| `CB-client/Assets/Scripts/States/GameOverState.cs` | Modify | Батч-dissolve врагов + прямой рестарт |

- [x] [Task 1: DissolveService + HealthSystem refactor](tasks/task-1.md)
- [x] [Task 2: GameContext + GameplayState wiring](tasks/task-2.md)
- [x] [Task 3: GameOverState — dissolve on enter + restart fix](tasks/task-3.md)

## What we did

- Создан `DissolveService` — статическая утилита, инкапсулирующая полный цикл уничтожения врага: снятие с карты занятости, отключение коллайдера, запуск dissolve-анимации и возврат в пул по её завершении.
- Из `HealthSystem` убрана inline-логика ожидания dissolve; метод теперь делегирует в `DissolveService`.
- `GameContext` получил свойство `EnemySpawnSystem`; `GameplayState` устанавливает его при создании системы спауна.
- `GameOverState` при входе делает snapshot активных врагов и запускает их батч-dissolve параллельно с показом PostBattle-экрана.
- Рестарт матча теперь идёт напрямую в `GameplayState`: игровое поле и текущая позиция игрока сохраняются, здоровье и таймеры волн сбрасываются штатной инициализацией систем.
