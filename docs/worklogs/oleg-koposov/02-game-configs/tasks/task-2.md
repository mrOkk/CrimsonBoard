# Task 2: Wire into GameContext and EntryPoint

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Modify: `CB-client/Assets/Scripts/Core/EntryPoint.cs`

**Commit message:** `02 Wire GameConfig into GameContext and EntryPoint`

### Steps

1. В `GameContext.cs` добавить публичное свойство и принять конфиг через конструктор:
   ```csharp
   public GameConfig Config { get; }

   public GameContext(GameConfig config)
   {
       Instance = this;
       Config = config;
   }
   ```

2. В `EntryPoint.cs` добавить сериализованное поле и передать конфиг в контекст:
   ```csharp
   [SerializeField] private GameConfig _config;

   private void Awake()
   {
       var context = new GameContext(_config);
       _fsm = new GameStateMachine();
       _fsm.ChangeState(new InitState(context, _fsm));
   }
   ```
   Существующий `private GameStateMachine _fsm;` остаётся на месте.

3. Проверить, что `InitState` и другие состояния, которые принимают `GameContext`, компилируются без изменений (конструктор GameContext теперь требует аргумент).

## Implementation
**Status:** DONE
**Summary:** `GameContext` получил публичное свойство `Config` и конструктор с параметром `GameConfig config`. `EntryPoint` получил `[SerializeField] private GameConfig _config` и передаёт конфиг в конструктор контекста.
