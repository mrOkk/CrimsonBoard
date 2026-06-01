# Task 2: GameContext + GameplayState wiring

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** `14 Add EnemySpawnSystem to GameContext, wire in GameplayState`

### Steps

1. **Добавить свойство `EnemySpawnSystem` в `GameContext`** рядом с `GameFieldSystem`:
   ```csharp
   public EnemySpawnSystem EnemySpawnSystem { get; set; }
   ```

2. **В конструкторе `GameplayState`** после строки `_enemySpawnSystem = new EnemySpawnSystem(context);`
   добавить присвоение:
   ```csharp
   context.EnemySpawnSystem = _enemySpawnSystem;
   ```
   Это гарантирует, что при входе в `GameOverState` (после `GameplayState.Exit()`) контекст
   содержит актуальный список активных врагов.

## Implementation

**Status:** DONE
**Summary:** Добавлено свойство `EnemySpawnSystem` в `GameContext`; в конструкторе `GameplayState` после создания экземпляра добавлена строка `context.EnemySpawnSystem = _enemySpawnSystem`.
