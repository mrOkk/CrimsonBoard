# 03 Entity Monobeh Components

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `03` |
| `task_file` | `docs/tasks/03-entity-monobeh-components.md` |
| `branch` | `feature/03-entity-monobeh-components` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3`|
| `status` | `active` |
| `created_at` | `2026-05-23T05:25:59Z` |
| `updated_at` | `2026-05-23T05:53:41Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-23T05:53:41Z | 2 → 3 | Plan signed off |
| 2026-05-23T05:31:25Z | 1 → 2 | Discovery signed off |
| 2026-05-23T05:25:59Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach
Иерархия наследования MonoBehaviour с типизированными ссылками в PrefabsConfig.

- `EntityView : MonoBehaviour` — базовый класс сущности. `[SerializeField]` поля: `MeshFilter`, `Rigidbody`, `Collider`. Без логики — чистая заготовка.
- `PlayerView : EntityView` — добавляет `[SerializeField] Transform _weaponLocator` (локатор оружия).
- `EnemyView : EntityView` — выделенный тип без доп. полей.
- `WeaponView : MonoBehaviour` — `[SerializeField]` поля: `MeshFilter`, `_playerAttachPoint`, `_muzzlePoint`.
- `PowerUpView : MonoBehaviour` — `[SerializeField]` поля: `SpriteRenderer`, `Collider`.
- `BoardTileView : MonoBehaviour` — `[SerializeField]` поле: `MeshFilter`.
- `PrefabsConfig` — заменить `GameObject playerPrefab/enemyPrefab/weaponPrefab/powerUpPrefab/tilePrefab` → типизированные MonoBehaviour. `projectilePrefab` остаётся `GameObject`.

### Scope

**In scope:**
- 6 C# MonoBehaviour-классов в `Scripts/Entities/`
- Замена 5 из 6 полей `PrefabsConfig` на типизированные ссылки

**Out of scope:**
- Создание .prefab-файлов (отдельная задача)
- Логика поведения (движение, стрельба и т.д.)
- `projectilePrefab` — тип не меняется

### Key Constraints
- Unity 2022.3 LTS, namespace `CrimsonBoard`
- Нет .asmdef — используется Assembly-CSharp по умолчанию
- Все компоненты-заготовки без логики, только сериализуемые поля и RequireComponent

### Files to Touch
- `CB-client/Assets/Scripts/Entities/EntityView.cs` *(new)*
- `CB-client/Assets/Scripts/Entities/PlayerView.cs` *(new)*
- `CB-client/Assets/Scripts/Entities/EnemyView.cs` *(new)*
- `CB-client/Assets/Scripts/Entities/WeaponView.cs` *(new)*
- `CB-client/Assets/Scripts/Entities/PowerUpView.cs` *(new)*
- `CB-client/Assets/Scripts/Entities/BoardTileView.cs` *(new)*
- `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs` *(edit)*

## Tasks

**Goal:** Создать шесть MonoBehaviour-заготовок для игровых сущностей (EntityView, PlayerView, EnemyView, WeaponView, PowerUpView, BoardTileView) и заменить нетипизированные `GameObject`-ссылки в `PrefabsConfig` на соответствующие типы — подготовив тем самым единообразную структуру для дальнейшей интеграции с конфигами и пулами.

**Architecture:** Базовый класс `EntityView : MonoBehaviour` аккумулирует компоненты общей сущности через `[RequireComponent]`. `PlayerView` и `EnemyView` наследуют от него. `WeaponView`, `PowerUpView`, `BoardTileView` — самостоятельные MonoBehaviour без общего предка. `PrefabsConfig` переходит от `GameObject` к типизированным ссылкам, что позволяет получать нужный компонент без `GetComponent<>` при инстанцировании.

**File structure:**
| Path | Type | Purpose |
|---|---|---|
| `CB-client/Assets/Scripts/Entities/EntityView.cs` | Create | Базовый класс сущности: RequireComponent(MeshFilter, Rigidbody, Collider) |
| `CB-client/Assets/Scripts/Entities/PlayerView.cs` | Create | Игрок: наследует EntityView, добавляет _weaponLocator |
| `CB-client/Assets/Scripts/Entities/EnemyView.cs` | Create | Враг: наследует EntityView, выделенный тип |
| `CB-client/Assets/Scripts/Entities/WeaponView.cs` | Create | Оружие: RequireComponent(MeshFilter), _playerAttachPoint, _muzzlePoint |
| `CB-client/Assets/Scripts/Entities/PowerUpView.cs` | Create | Павер-ап: RequireComponent(SpriteRenderer, Collider) |
| `CB-client/Assets/Scripts/Entities/BoardTileView.cs` | Create | Тайл: RequireComponent(MeshFilter) |
| `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs` | Modify | Замена GameObject → типизированных MonoBehaviour для 5 полей |

- [ ] [Task 1: Entity view scripts](tasks/task-1.md)
- [ ] [Task 2: Update PrefabsConfig](tasks/task-2.md)

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
