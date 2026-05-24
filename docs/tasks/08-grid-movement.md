# 08: grid-movement

## Description

Подготовить задачу на систему передвижения игровых сущностей по клеткам.

Требования:
- игровые сущности (игрок и враги) умеют перемещаться по сетке;
- перемещение выполняется из клетки A в клетку B;
- на этом этапе без анимационных "красивостей" и интерполяций;
- допускается мгновенное логическое перемещение в целевую клетку;
- перемещение запрещено в занятые/недоступные клетки;
- движение должно быть детерминированным и тик-ориентированным.

Интеграции:
- с `game-field-streaming`: работа в координатах клеток/чанков;
- с `health-system`: корректная фиксация входа врага в клетку игрока;
- с `entity-monobeh-components`: применение к игроку и врагам.

Ожидаемый результат:
- `GridMovementSystem` для перемещения сущностей по клеткам;
- общий контракт движения (например, `MoveRequest`, `MoveResult`);
- проверка коллизий/занятости целевой клетки;
- заготовка для расширения на анимационное движение в будущих задачах.

## Comments

**[Copilot, 2026-05-24]:** Discovery complete. Approach: `GridMovementSystem` (TryMove + OccupancyMap/HealthSystem интеграция) + `PlayerMovementSystem` (InputSystem_Actions, 8 направлений, задержка beatDuration/movesPerBeat, indicator). Scope: in — оба системы, EntityView.CurrentCell, PlayerView.DirectionIndicator, ChunkCoordConverter tile methods, PlayerSpawnSystem OccupancyMap wiring, HealthSystem knockback transform fix; out — AI врагов, визуальная интерполяция, beat system.
