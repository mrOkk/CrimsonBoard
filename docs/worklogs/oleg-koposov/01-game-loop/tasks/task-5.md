# Task 5: Five state stubs

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/States/InitState.cs` + `.meta`
- Create: `CB-client/Assets/Scripts/States/TapToStartState.cs` + `.meta`
- Create: `CB-client/Assets/Scripts/States/GameplayState.cs` + `.meta`
- Create: `CB-client/Assets/Scripts/States/PauseState.cs` + `.meta`
- Create: `CB-client/Assets/Scripts/States/GameOverState.cs` + `.meta`

**Commit message:** `01 Add five game state stubs`

### Steps

1. Create `InitState.cs` — runs once on startup. On `Enter`, perform initialization
   (resource loading, config setup, UI init). On complete, transitions to `TapToStartState`.
   Stub: log a message in `Enter`, transition immediately (no real loading yet).
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class InitState : IGameState
       {
           private readonly GameContext _context;
           private readonly GameStateMachine _fsm;

           public InitState(GameContext context, GameStateMachine fsm)
           {
               _context = context;
               _fsm = fsm;
           }

           public void Enter()
           {
               Debug.Log("[InitState] Enter");
               // TODO: load resources, init UI, load configs
               _fsm.ChangeState(new TapToStartState(_context, _fsm));
           }

           public void Exit() => Debug.Log("[InitState] Exit");

           public void Tick(float deltaTime) { }
       }
   }
   ```

2. Create `TapToStartState.cs` — waits for player input to begin gameplay.
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class TapToStartState : IGameState
       {
           private readonly GameContext _context;
           private readonly GameStateMachine _fsm;

           public TapToStartState(GameContext context, GameStateMachine fsm)
           {
               _context = context;
               _fsm = fsm;
           }

           public void Enter() => Debug.Log("[TapToStartState] Enter");
           public void Exit() => Debug.Log("[TapToStartState] Exit");

           public void Tick(float deltaTime)
           {
               // TODO: detect tap/click and transition to GameplayState
           }
       }
   }
   ```

3. Create `GameplayState.cs` — core gameplay; owns a `GameplaySystemRunner`.
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class GameplayState : IGameState
       {
           private readonly GameContext _context;
           private readonly GameStateMachine _fsm;
           private readonly GameplaySystemRunner _systemRunner;

           public GameplayState(GameContext context, GameStateMachine fsm)
           {
               _context = context;
               _fsm = fsm;
               _systemRunner = new GameplaySystemRunner();
           }

           public void Enter()
           {
               Debug.Log("[GameplayState] Enter");
               _systemRunner.Initialize();
           }

           public void Exit()
           {
               Debug.Log("[GameplayState] Exit");
               _systemRunner.Dispose();
           }

           public void Tick(float deltaTime) => _systemRunner.Tick(deltaTime);
       }
   }
   ```

4. Create `PauseState.cs` — pauses gameplay; resumes to previous state.
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class PauseState : IGameState
       {
           private readonly GameContext _context;
           private readonly GameStateMachine _fsm;

           public PauseState(GameContext context, GameStateMachine fsm)
           {
               _context = context;
               _fsm = fsm;
           }

           public void Enter() => Debug.Log("[PauseState] Enter");
           public void Exit() => Debug.Log("[PauseState] Exit");

           public void Tick(float deltaTime)
           {
               // TODO: handle settings changes, resume input
           }
       }
   }
   ```

5. Create `GameOverState.cs` — shows win/death screen; allows restart.
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class GameOverState : IGameState
       {
           private readonly GameContext _context;
           private readonly GameStateMachine _fsm;

           public GameOverState(GameContext context, GameStateMachine fsm)
           {
               _context = context;
               _fsm = fsm;
           }

           public void Enter() => Debug.Log("[GameOverState] Enter");
           public void Exit() => Debug.Log("[GameOverState] Exit");

           public void Tick(float deltaTime)
           {
               // TODO: detect restart input → ChangeState(new TapToStartState(...))
           }
       }
   }
   ```

6. Create `.meta` files for all five `.cs` files using the MonoImporter template (see Task 2 for format).

7. Commit all new files.

## Implementation
**Status:** DONE
**Summary:** Created all five state stubs (InitState, TapToStartState, GameplayState, PauseState, GameOverState) each implementing IGameState with constructor injection of GameContext and GameStateMachine; GameplayState owns a GameplaySystemRunner.
