# Task 3: GameContext singleton

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/GameContext.cs` + `.meta`

**Commit message:** `01 Add GameContext singleton`

### Steps

1. Create `GameContext.cs` in `CB-client/Assets/Scripts/Core/`:
   ```csharp
   namespace CrimsonBoard
   {
       /// <summary>
       /// Central dependency container. Passed to every game state.
       /// Add typed fields as systems are implemented in future tasks.
       /// </summary>
       public class GameContext
       {
           public static GameContext Instance { get; private set; }

           public GameContext()
           {
               Instance = this;
           }
       }
   }
   ```

2. Create `.meta` file for `GameContext.cs` using the MonoImporter template (see Task 2 for format).

3. Commit all new files.

## Implementation
**Status:** DONE
**Summary:** Created GameContext.cs singleton with static Instance property; sets itself on construction.
