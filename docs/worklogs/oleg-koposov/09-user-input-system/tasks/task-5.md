# Task 5: Refactor PlayerMovementSystem + wire GameplayState

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Systems/PlayerMovementSystem.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** `09 Refactor PlayerMovementSystem to consume InputState; wire new systems`

### Steps

1. **Rewrite `PlayerMovementSystem.cs`** — remove all `InputSystem_Actions` usage. The system now only reads `_context.InputState.MoveCommand` and delegates movement to `GridMovementSystem`. Keep the `RoundToGridDir` helper in `PlayerInputSystem` (already moved in Task 4), remove it from here. The cooldown logic and `DirectionIndicator` rotation are unchanged:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class PlayerMovementSystem : IGameSystem
       {
           private readonly GameContext _context;
           private readonly GridMovementSystem _gridMovement;
           private float _cooldownRemaining;

           public PlayerMovementSystem(GameContext context, GridMovementSystem gridMovement)
           {
               _context = context;
               _gridMovement = gridMovement;
           }

           public void Initialize()
           {
               _cooldownRemaining = 0f;
           }

           public void Tick(float deltaTime)
           {
               _cooldownRemaining -= deltaTime;
               if (_cooldownRemaining > 0f) return;

               var cmd = _context.InputState.MoveCommand;
               if (cmd == null) return;

               var result = _gridMovement.TryMove(_context.Player, cmd.Value);
               if (result == MoveResult.Moved)
               {
                   var timing = _context.Config.timing;
                   _cooldownRemaining = timing.beatDuration / Mathf.Max(1, _context.Config.player.movesPerBeat);

                   if (_context.Player.DirectionIndicator != null)
                       _context.Player.DirectionIndicator.rotation =
                           Quaternion.LookRotation(new Vector3(cmd.Value.x, 0f, cmd.Value.y), Vector3.up);
               }
           }

           public void Dispose() { }
       }
   }
   ```

2. **Update `GameplayState.cs`** — add `PlayerInputSystem` registration **before** `PlayerMovementSystem`, and `HopAnimationSystem` registration **after** all movement systems. No other changes to existing system registrations:
   ```csharp
   // Add at the top of the constructor, before other system registrations:
   _systemRunner.RegisterSystem(new PlayerInputSystem(context));

   // Keep existing registrations unchanged:
   _systemRunner.RegisterSystem(context.GameFieldSystem);
   _systemRunner.RegisterSystem(new CameraFollowSystem(context));
   _healthSystem = new HealthSystem(context, fsm);
   _gridMovementSystem.HealthSystem = _healthSystem;
   _systemRunner.RegisterSystem(_healthSystem);
   _systemRunner.RegisterSystem(_gridMovementSystem);
   _systemRunner.RegisterSystem(new PlayerMovementSystem(context, _gridMovementSystem));

   // Add at the end:
   _systemRunner.RegisterSystem(new HopAnimationSystem(context));
   ```

## Implementation
<!-- Filled in Phase 3 -->
