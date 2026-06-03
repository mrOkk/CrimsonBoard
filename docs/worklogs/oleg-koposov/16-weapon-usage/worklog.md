# 16 weapon usage

| Field | Value |
|---|---|
| `workflow_version` | `1` |
| `ticket_id` | `16` |
| `task_file` | `docs/tasks/16-weapon-usage.md` |
| `branch` | `feature/16-weapon-usage` |
| `user_key` | `oleg-koposov` |
| `current_phase` | `3` |
| `status` | `done` |
| `created_at` | `2026-06-03T19:31:58Z` |
| `updated_at` | `2026-06-03T19:35:00Z` |

## Phase History

| When | From → To | Note |
|---|---|---|
| 2026-06-03T19:31:58Z | — → 1 | Created by wf-start |
| 2026-06-03T19:32:30Z | 1 → 2 | Discovery signed off |
| 2026-06-03T19:35:00Z | 2 → 3 | Plan approved, implementation complete |

## Discovery

### Chosen Approach

Создать `WeaponUsageSystem` как отдельную систему, которая управляет:
1. Отображением/скрытием оружий на игроке
2. Анимацией смены оружия (holster/draw с вращением)
3. Поворотом игрока к ближайшему врагу
4. Автоматической стрельбой по ритму

### Scope

**In:**
- Расширение `WeaponConfig`: range, holsterTime, drawTime, maxTargetsPerBullet
- Расширение `WeaponView`: rotationPoint (Transform)
- Реализация `ProjectileView`: движение, trigger collider, piercing
- Новый `WeaponUsageSystem` — управление оружием игрока
- Расширение `PlayerInventory`: Next/Previous с пропуском пустых
- Модификация `PlayerInputSystem`: добавить Next/Previous weapon, удалить закомментированный код атаки
- Добавить Next/Previous в InputSystem_Actions (если нет)

**Out:**
- UI отображение оружия (отдельная задача)
- Звуки оружия
- Разные типы снарядов (пока один базовый)

### Design Overview

**WeaponUsageSystem (IGameSystem):**
- Хранит список `WeaponView` активных оружий на игроке
- `Tick`: проверяет ближайшего врага в range, инициирует поворот и стрельбу
- Смена оружия через корутину: скрыть текущее (holster анимация), показать новое (draw анимация)
- Стрельба привязана к `TimingConfig.beatDuration / shotsPerBeat`

**ProjectileView:**
- `Launch(dir, speed, damage, pierceCount, range)` — движение вперёд
- Trigger collider для попаданий
- При попадании: наносит урон, уменьшает pierceCount, если 0 — возвращается в пул

**WeaponView:**
- `rotationPoint` — точка для анимации holster/draw (вращение вокруг X)
- Вращение всего transform оружия вниз при holster, вверх при draw

**Смена оружия:**
- Инпут Next/Previous переключает `PlayerInventory.ActiveWeaponId`
- Циклический переход с пропуском оружий без патронов
- Анимация через `WeaponUsageSystem.StartCoroutine`

### Files to Touch

- `Core/Configs/WeaponConfig.cs` — добавить поля
- `Core/Systems/WeaponUsageSystem.cs` — новый файл
- `Core/Inventory/PlayerInventory.cs` — методы Next/Previous
- `Core/Systems/PlayerInputSystem.cs` — инпут смены, удалить комментарии
- `Entities/WeaponView.cs` — rotationPoint
- `Entities/ProjectileView.cs` — реализация
- `Settings/InputSystem_Actions.inputactions` — если нет Next/Previous (проверить)

## Tasks

### Goal
Реализовать использование оружия игроком: отображение, смена с анимацией, автоматическая стрельба по ближайшему врагу, снаряды с пробиванием.

### Architecture
- **WeaponUsageSystem** — новая IGameSystem, управляет оружием на игроке
- **ProjectileView** — движущийся снаряд с trigger collider и piercing
- **PlayerInventory** — расширение методами Next/Previous

### File Structure
```
Core/Configs/WeaponConfig.cs       — добавить поля
Core/Systems/WeaponUsageSystem.cs  — новый файл
Core/Inventory/PlayerInventory.cs  — добавить методы
Core/Systems/PlayerInputSystem.cs   — инпут смены, удалить комментарии
Entities/WeaponView.cs              — добавить rotationPoint
Entities/ProjectileView.cs          — реализация
```

### Task List
- [ ] Task 1: Extend WeaponConfig — [task-1.md](tasks/task-1.md)
- [ ] Task 2: Add rotationPoint to WeaponView — [task-2.md](tasks/task-2.md)
- [ ] Task 3: Implement ProjectileView — [task-3.md](tasks/task-3.md)
- [ ] Task 4: Add Next/Previous to PlayerInventory — [task-4.md](tasks/task-4.md)
- [ ] Task 5: Create WeaponUsageSystem — [task-5.md](tasks/task-5.md)
- [ ] Task 6: Update PlayerInputSystem — [task-6.md](tasks/task-6.md)
- [ ] Task 7: Wire WeaponUsageSystem into GameplayState — [task-7.md](tasks/task-7.md)

## What we did

- Extended `WeaponConfig` with `range`, `holsterTime`, `drawTime`, `maxTargetsPerBullet`
- Added `rotationPoint` to `WeaponView` for holster/draw animation pivot
- Implemented `ProjectileView` with movement, piercing, and damage application
- Added `CycleNext/CyclePrevious` to `PlayerInventory` with ammo skip logic
- Created `WeaponUsageSystem` managing weapon display, switching animation, aiming, and automatic fire
- Updated `PlayerInputSystem` to handle Next/Previous input, removed commented attack code
- Wired `WeaponUsageSystem` into `GameplayState`
