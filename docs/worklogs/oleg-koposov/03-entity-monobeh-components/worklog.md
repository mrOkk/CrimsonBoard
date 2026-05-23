# 03 Entity Monobeh Components

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `03` |
| `task_file` | `docs/tasks/03-entity-monobeh-components.md` |
| `branch` | `feature/03-entity-monobeh-components` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `2`|
| `status` | `active` |
| `created_at` | `2026-05-23T05:25:59Z` |
| `updated_at` | `2026-05-23T05:31:25Z`|

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-05-23T05:31:25Z | 1 → 2 | Discovery signed off |
| 2026-05-23T05:25:59Z | — → 1 | Created by wf-start |

## Discovery

### Chosen Approach
Иерархия наследования MonoBehaviour с типизированными ссылками в PrefabsConfig.

- `EntityView : MonoBehaviour` — базовый класс сущности. `[RequireComponent(MeshFilter, Rigidbody, Collider)]`. Без логики — чистая заготовка.
- `PlayerView : EntityView` — добавляет `[SerializeField] Transform _weaponLocator` (локатор оружия).
- `EnemyView : EntityView` — выделенный тип без доп. полей.
- `WeaponView : MonoBehaviour` — `[RequireComponent(MeshFilter)]` + `[SerializeField] Transform _playerAttachPoint, _muzzlePoint`.
- `PowerUpView : MonoBehaviour` — `[RequireComponent(SpriteRenderer, Collider)]`.
- `BoardTileView : MonoBehaviour` — `[RequireComponent(MeshFilter)]`.
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

<!-- Filled by wf-plan: cross-cutting Goal/Architecture/File-structure headers
     + checkbox list of Tasks linking to tasks/task-<N>.md. -->

## What we did

<!-- Filled manually or by wf-implement at phase end. -->
