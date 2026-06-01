# 14 Battle End Dissolve

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `14` |
| `task_file` | `docs/tasks/14-battle-end-dissolve.md` |
| `branch` | `feature/14-battle-end-dissolve` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-06-01T19:55:22Z` |
| `updated_at` | `2026-06-01T20:01:17Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
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

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
