# 07 Health System

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `07` |
| `task_file` | `docs/tasks/07-health-system.md` |
| `branch` | `feature/07-health-system` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-24T18:46:28Z` |
| `updated_at` | `2026-05-24T19:04:27Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-24T19:04:27Z | 2 → 3 | Plan signed off |
| 2026-05-24T19:01:08Z | 1 → 2 | Discovery signed off |
| 2026-05-24T18:46:28Z | — → 1 | Created by wf-start |

## Discovery

**Подход:** Scaffold-реализация с правильной архитектурой и TODO-комментариями для интеграции с движением врагов (которое будет в следующих задачах).

**Архитектура:**
- `HealthComponent : MonoBehaviour` — добавляется на PlayerView/EnemyView; хранит `float maxHp`, `float currentHp`; методы `TakeDamage(float)`, `Heal(float)`, `IsDead`; событие `System.Action OnDeath`
- `OccupancyMap` — plain C# класс в GameContext; `Dictionary<Vector2Int, EntityView>`; методы Register/Unregister/IsOccupied/GetEntity
- `KnockbackResolver` — static class; `Resolve(Vector2Int playerPos, Vector2Int enemyDir, OccupancyMap map) → Vector2Int?`; приоритет: клетка напротив движения врага, затем фиксированный порядок соседей (N/S/E/W)
- `HealthSystem : IGameSystem` — принимает `GameContext` + `GameStateMachine`; при вызове `ApplyDamageToPlayer(EnemyView enemy)` применяет урон, вычисляет knockback через KnockbackResolver, при `IsDead` переходит в `GameOverState`; при смерти врага: Unregister из OccupancyMap + `context.Pools.Enemies.Return(enemy)`
- Конфиги: `int health` → `float health` в `PlayerConfig` и `EnemyConfig`

**Scope in:** HealthComponent, OccupancyMap, KnockbackResolver, HealthSystem, обновление PlayerConfig/EnemyConfig (int→float), добавление HealthComponent-ссылки в PlayerView/EnemyView, регистрация HealthSystem в GameplayState  
**Scope out:** движение врагов, реальный триггер коллизии по клеткам, анимации, UI HP-бара, звук

**Ключевые ограничения:**
- HP не уходит в отрицательные значения (clamp to 0)
- Knockback детерминирован (фиксированный порядок fallback-клеток)
- Нельзя переместить игрока в клетку с врагом
- Если все соседние клетки заняты — только урон, без перемещения

**Файлы:**
- `Core/Configs/PlayerConfig.cs` — int→float health
- `Core/Configs/EnemyConfig.cs` — int→float health
- `Entities/PlayerView.cs` — добавить [SerializeField] HealthComponent
- `Entities/EnemyView.cs` — добавить [SerializeField] HealthComponent
- `Entities/HealthComponent.cs` — new MonoBehaviour
- `Core/GameContext.cs` — добавить OccupancyMap
- `Core/OccupancyMap.cs` — new class
- `Core/Systems/KnockbackResolver.cs` — new static class
- `Core/Systems/HealthSystem.cs` — new IGameSystem
- `States/GameplayState.cs` — регистрация HealthSystem

## Tasks

**Goal:** Реализовать scaffold системы здоровья для игрока и врагов. Задача разбита на три слоя: данные и компоненты → карта занятости и resolver → система здоровья и её регистрация.

**Architecture:** `HealthComponent` (MonoBehaviour) хранит float HP и генерирует событие OnDeath. `OccupancyMap` в GameContext отслеживает, кто стоит на какой клетке. `KnockbackResolver` детерминированно выбирает клетку отбрасывания. `HealthSystem` соединяет их вместе и при смерти игрока переводит FSM в GameOverState.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `Core/Configs/PlayerConfig.cs` | Modify | int→float health |
| `Core/Configs/EnemyConfig.cs` | Modify | int→float health, int→float damage |
| `Entities/HealthComponent.cs` | Create | MonoBehaviour с float HP, событием OnDeath |
| `Entities/PlayerView.cs` | Modify | добавить HealthComponent property |
| `Entities/EnemyView.cs` | Modify | добавить HealthComponent property + Setup(EnemyConfig) |
| `Core/OccupancyMap.cs` | Create | Dictionary<Vector2Int, EntityView>, Register/Unregister/IsOccupied |
| `Core/GameContext.cs` | Modify | добавить OccupancyMap property |
| `Core/Systems/KnockbackResolver.cs` | Create | static Resolve() с детерминированным fallback |
| `Core/Systems/HealthSystem.cs` | Create | IGameSystem, ApplyDamageToPlayer, OnEnemyDeath |
| `States/GameplayState.cs` | Modify | регистрация HealthSystem |

- [x] [Task 1: Data layer — configs, HealthComponent, View wiring](tasks/task-1.md)
- [ ] [Task 2: OccupancyMap, GameContext, KnockbackResolver](tasks/task-2.md)
- [ ] [Task 3: HealthSystem and GameplayState wiring](tasks/task-3.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
