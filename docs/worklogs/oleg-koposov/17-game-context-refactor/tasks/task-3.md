# Task 3: Migrate EnemySpawnSystem out of GameContext

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`
- Modify: `CB-client/Assets/Scripts/States/GameOverState.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/HopAnimationSystem.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/WeaponUsageSystem.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`

**Commit message:** 17 migrate EnemySpawnSystem to GameBoard

### Steps

1. В `GameplayState.cs`, создать `EnemySpawnSystem` локально и обновить `_context.Board`:
   ```csharp
   private EnemySpawnSystem _enemySpawnSystem;

   public GameplayState(GameContext context, GameStateMachine fsm)
   {
       _context = context;
       _fsm = fsm;
       _systemRunner = new GameplaySystemRunner();

       _enemySpawnSystem = new EnemySpawnSystem(context);
   }

   public void Enter()
   {
       _context.Board.RegisterEnemySpawnSystem(_enemySpawnSystem);
       // ... rest of initialization
   }
   ```

2. Обновить `GameBoard`, добавить метод для регистрации EnemySpawnSystem:
   ```csharp
   private EnemySpawnSystem _enemySpawnSystem;

   public void RegisterEnemySpawnSystem(EnemySpawnSystem system)
   {
       _enemySpawnSystem = system;
   }

   public IReadOnlyList<EnemyView> GetActiveEnemies()
       => _enemySpawnSystem?.ActiveEnemies ?? Array.Empty<EnemyView>().ToList();
   ```

3. В `GameOverState.cs`, заменить `_context.EnemySpawnSystem` на `_context.Board.GetActiveEnemies()`:
   ```csharp
   var enemies = _context.Board?.GetActiveEnemies();
   ```

4. В `HopAnimationSystem.cs`, изменить конструктор для приёма `GameBoard`:
   ```csharp
   public HopAnimationSystem(GameContext context, GameBoard board)
   {
       _context = context;
       _board = board;
   }

   public void Tick(float deltaTime)
   {
       if (_context.Player?.IsHopping == true)
           _context.Player.UpdateHop(deltaTime);

       foreach (var enemy in _board.GetActiveEnemies())
       {
           if (enemy.IsHopping)
               enemy.UpdateHop(deltaTime);
       }
   }
   ```

5. В `WeaponUsageSystem.cs`, заменить `_context.EnemySpawnSystem` на `_context.Board.GetActiveEnemies()`:
   ```csharp
   public void TryUseWeapon(WeaponConfig config, Vector3 targetPosition)
   {
       var enemies = _context.Board?.GetActiveEnemies();
       if (enemies == null || enemies.Count == 0)
           return;

       var nearestEnemy = FindNearestEnemy(enemies, targetPosition);
       // ... rest of logic
   }
   ```

6. Удалить `public EnemySpawnSystem EnemySpawnSystem { get; set; }` из `GameContext.cs`.

7. Проверить что нет других ссылок на `context.EnemySpawnSystem` (grep).

## Implementation
**Status:** DONE
**Summary:** Migrated ActiveEnemies from EnemySpawnSystem to GameBoard. HopAnimationSystem now receives GameBoard instead of EnemySpawnSystem.
