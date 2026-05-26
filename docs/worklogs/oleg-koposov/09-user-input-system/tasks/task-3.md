# Task 3: HopAnimationSystem

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/HopAnimationSystem.cs`

**Commit message:** `09 Add HopAnimationSystem`

### Steps

1. **Create `HopAnimationSystem.cs`** in `Core/Systems/`. Follows the same `IGameSystem` pattern as `CameraFollowSystem`. For now ticks only the player; enemy support is added in a future task when enemy movement is hooked up:
   ```csharp
   namespace CrimsonBoard
   {
       public class HopAnimationSystem : IGameSystem
       {
           private readonly GameContext _context;

           public HopAnimationSystem(GameContext context)
           {
               _context = context;
           }

           public void Initialize() { }

           public void Tick(float deltaTime)
           {
               _context.Player?.TickHop(deltaTime);
           }

           public void Dispose() { }
       }
   }
   ```

## Implementation
**Status:** DONE
**Summary:** Created `HopAnimationSystem` exactly as planned. Ticks `_context.Player?.TickHop(deltaTime)` each frame.
