# Task 7: EntryPoint MonoBehaviour

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/EntryPoint.cs` + `.meta`

**Commit message:** `01 Add EntryPoint MonoBehaviour`

### Steps

1. Create `EntryPoint.cs` in `CB-client/Assets/Scripts/Core/`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       /// <summary>
       /// Bootstrap MonoBehaviour. Place on the root GameObject of the main scene.
       /// Creates GameContext and GameStateMachine, then kicks off InitState.
       /// </summary>
       public class EntryPoint : MonoBehaviour
       {
           private GameStateMachine _fsm;

           private void Awake()
           {
               var context = new GameContext();
               _fsm = new GameStateMachine();
               _fsm.ChangeState(new InitState(context, _fsm));
           }

           private void Update()
           {
               _fsm.Tick(Time.deltaTime);
           }
       }
   }
   ```

2. Create `.meta` file for `EntryPoint.cs` using the MonoImporter template (see Task 2 for format).

3. Commit all new files.

## Implementation
**Status:** DONE
**Summary:** Created EntryPoint.cs MonoBehaviour; Awake creates GameContext and GameStateMachine then enters InitState; Update ticks the FSM each frame.
