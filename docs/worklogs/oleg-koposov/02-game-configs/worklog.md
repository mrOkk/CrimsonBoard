# 02 Game Configs

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `02` |
| `task_file` | `docs/tasks/02-game-configs.md` |
| `branch` | `feature/02-game-configs` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-20T05:18:57Z` |
| `updated_at` | `2026-05-20T05:31:30Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-20T05:31:30Z | 1 → 2 | Discovery signed off |
| 2026-05-20T05:18:57Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach

Один `GameConfig : ScriptableObject` с атрибутом `[CreateAssetMenu]`. Данные каждой сущности вынесены в отдельные `[Serializable]`-классы в папке `Core/Configs/`.

`GameContext` хранит ссылку на `GameConfig` (передаётся через конструктор).  
`EntryPoint` получает конфиг через `[SerializeField]`-поле и прокидывает его в контекст при старте.

### Scope

**In:**
- C#-скрипты конфига (`GameConfig`, `PlayerConfig`, `EnemyConfig`, `WeaponConfig`, `TimingConfig`)
- Интеграция в `GameContext` и `EntryPoint`

**Out:**
- Создание `.asset`-файла (делается вручную в Unity через Create-меню)
- Логика игры, использующая значения конфига

### Key Constraints

- Скорость передвижения — количество клеток в такт (не units/sec)
- Меши врагов и оружия — массивы вариантов (`Mesh[]`)
- Все поля публичные для сериализации в инспекторе Unity

### Files to Touch

| Action | Path |
|---|---|
| NEW | `CB-client/Assets/Scripts/Core/Configs/GameConfig.cs` |
| NEW | `CB-client/Assets/Scripts/Core/Configs/PlayerConfig.cs` |
| NEW | `CB-client/Assets/Scripts/Core/Configs/EnemyConfig.cs` |
| NEW | `CB-client/Assets/Scripts/Core/Configs/WeaponConfig.cs` |
| NEW | `CB-client/Assets/Scripts/Core/Configs/TimingConfig.cs` |
| MOD | `CB-client/Assets/Scripts/Core/GameContext.cs` |
| MOD | `CB-client/Assets/Scripts/Core/EntryPoint.cs` |

## Tasks

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
