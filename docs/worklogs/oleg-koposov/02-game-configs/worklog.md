# 02 Game Configs

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `02` |
| `task_file` | `docs/tasks/02-game-configs.md` |
| `branch` | `feature/02-game-configs` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `done`|
| `created_at` | `2026-05-20T05:18:57Z` |
| `updated_at` | `2026-05-20T05:36:08Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-20T05:36:08Z | active → done | Implemented by wf-implement |
| 2026-05-20T05:33:54Z | 2 → 3 | Plan signed off |
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

**Goal:** Создать скаффолд ScriptableObject-конфига для всех ключевых сущностей игры (игрок, враг, оружие, павер-ап, проджектайл, тайминги) и подключить его к `GameContext` через `EntryPoint`.

**Architecture:** Один `GameConfig : ScriptableObject` агрегирует четыре `[Serializable]`-структуры (`PlayerConfig`, `EnemyConfig`, `WeaponConfig`, `TimingConfig`) и два прямых поля-префаба для павер-апа и проджектайла. `GameContext` хранит `GameConfig` как публичное свойство, `EntryPoint` получает его через `[SerializeField]` и передаёт в конструктор контекста.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Core/Configs/PlayerConfig.cs` | Create | [Serializable] данные игрока |
| `CB-client/Assets/Scripts/Core/Configs/EnemyConfig.cs` | Create | [Serializable] данные врага |
| `CB-client/Assets/Scripts/Core/Configs/WeaponConfig.cs` | Create | [Serializable] данные оружия |
| `CB-client/Assets/Scripts/Core/Configs/TimingConfig.cs` | Create | [Serializable] тайминги такта/фазы |
| `CB-client/Assets/Scripts/Core/Configs/GameConfig.cs` | Create | ScriptableObject-контейнер всех конфигов |
| `CB-client/Assets/Scripts/Core/GameContext.cs` | Modify | Добавить поле `Config` |
| `CB-client/Assets/Scripts/Core/EntryPoint.cs` | Modify | Добавить [SerializeField] и прокинуть в контекст |

- [x] [Task 1: Config scripts](tasks/task-1.md)
- [x] [Task 2: Wire into GameContext and EntryPoint](tasks/task-2.md)

## What we did

Добавлен ScriptableObject-конфиг `GameConfig`, доступный через меню *Create → CrimsonBoard → GameConfig* в Unity.
Конфиг содержит данные четырёх сущностей:
- **Игрок** — ссылка на меш, префаб, здоровье, количество движений в такт.
- **Враг** — набор вариантов мешей, префаб, здоровье, урон, движения в такт.
- **Оружие** — набор вариантов мешей, префаб, урон, скорострельность (выстрелов в такт), разброс, скорость поворота, коэффициент скорости движения.
- **Тайминги** — длительность такта и фазы.

Также хранит прямые ссылки на префабы павер-апа и проджектайла.

`GameContext` теперь принимает конфиг через конструктор и хранит его как публичное свойство.
`EntryPoint` получает ассет конфига через инспектор (перетащить `.asset`-файл в поле) и передаёт его в контекст при старте.


