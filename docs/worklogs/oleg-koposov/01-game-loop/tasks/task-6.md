# Task 6: GameplaySystemRunner

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Gameplay/GameplaySystemRunner.cs` + `.meta`

**Commit message:** `01 Add GameplaySystemRunner`

### Steps

1. Create `GameplaySystemRunner.cs` in `CB-client/Assets/Scripts/Gameplay/`:
   ```csharp
   using System.Collections.Generic;

   namespace CrimsonBoard
   {
       /// <summary>
       /// Owns and drives a list of IGameSystem objects.
       /// Called by GameplayState each frame.
       /// Add systems via RegisterSystem() before Initialize().
       /// </summary>
       public class GameplaySystemRunner
       {
           private readonly List<IGameSystem> _systems = new List<IGameSystem>();

           public void RegisterSystem(IGameSystem system)
           {
               _systems.Add(system);
           }

           public void Initialize()
           {
               foreach (var system in _systems)
                   system.Initialize();
           }

           public void Tick(float deltaTime)
           {
               foreach (var system in _systems)
                   system.Tick(deltaTime);
           }

           public void Dispose()
           {
               foreach (var system in _systems)
                   system.Dispose();
               _systems.Clear();
           }
       }
   }
   ```

2. Create `.meta` file for `GameplaySystemRunner.cs` using the MonoImporter template (see Task 2 for format).

3. Commit all new files.

## Implementation

<!-- Filled in Phase 3 -->
