# Task 4: GameStateMachine

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/GameStateMachine.cs` + `.meta`

**Commit message:** `01 Add GameStateMachine`

### Steps

1. Create `GameStateMachine.cs` in `CB-client/Assets/Scripts/Core/`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class GameStateMachine
       {
           private IGameState _currentState;
           private IGameState _previousState;

           public IGameState CurrentState => _currentState;

           public void ChangeState(IGameState newState)
           {
               _previousState = _currentState;
               _currentState?.Exit();
               _currentState = newState;
               _currentState.Enter();
           }

           /// <summary>
           /// Transitions to PauseState from any post-init state.
           /// Call ResumePreviousState() to return.
           /// </summary>
           public void RequestPause(IGameState pauseState)
           {
               if (_currentState == pauseState) return;
               ChangeState(pauseState);
           }

           public void ResumePreviousState()
           {
               if (_previousState == null)
               {
                   Debug.LogWarning("[GameStateMachine] No previous state to resume.");
                   return;
               }
               ChangeState(_previousState);
           }

           public void Tick(float deltaTime)
           {
               _currentState?.Tick(deltaTime);
           }
       }
   }
   ```

2. Create `.meta` file for `GameStateMachine.cs` using the MonoImporter template (see Task 2 for format).

3. Commit all new files.

## Implementation

<!-- Filled in Phase 3 -->
