# Task 3: GameOverState — dissolve on enter + restart fix

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/States/GameOverState.cs`

**Commit message:** `14 GameOverState: dissolve active enemies, restart directly into GameplayState`

### Steps

1. **В `GameOverState.Enter()`** перед показом UI получить snapshot активных врагов
   и запустить батч-dissolve:
   ```csharp
   // Snapshot active enemies before GameplayState disposes the spawn system
   var enemies = _context.EnemySpawnSystem?.ActiveEnemies;
   if (enemies != null && enemies.Count > 0)
   {
       var snapshot = new System.Collections.Generic.List<EnemyView>(enemies);
       DissolveService.DissolveAllAndReturn(snapshot, _context.OccupancyMap, _context.Pools);
   }
   ```
   Поместить этот блок **до** `_context.UiRoot.Show<PostBattleView>()`.

2. **Изменить `OnRestart`** в `GameOverState.Enter()` — убрать `TapToStartState`,
   перейти напрямую в `GameplayState`:
   ```csharp
   view.OnRestart = () => _fsm.ChangeState(new GameplayState(_context, _fsm));
   ```
   (было: `new TapToStartState(_context, _fsm, autoStart: true)`)

   Это сохраняет игровое поле и позицию игрока.
   `GameplayState.Enter()` → `_systemRunner.Initialize()` сбрасывает здоровье игрока и таймеры волн.

## Implementation

**Status:** DONE
**Summary:** В `GameOverState.Enter()` добавлен snapshot активных врагов и вызов `DissolveService.DissolveAllAndReturn` перед показом UI; `OnRestart` теперь переходит в `new GameplayState(...)` напрямую, минуя `TapToStartState`.
