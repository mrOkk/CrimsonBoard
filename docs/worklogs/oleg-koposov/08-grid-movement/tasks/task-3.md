# Task 3: PlayerMovementSystem — input, cooldown, 8-dir, direction indicator

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Systems/PlayerMovementSystem.cs`

**Commit message:** `08 Add PlayerMovementSystem with input, cooldown and direction indicator`

### Steps

1. Создать `Core/Systems/PlayerMovementSystem.cs`. Система:
   - Создаёт `InputSystem_Actions` в Initialize, включает `Player` action map
   - В Tick читает `Move` action value как Vector2, конвертирует в 8-directional dir, применяет кулдаун
   - При успешном TryMove поворачивает `_directionIndicator` на PlayerView

   ```csharp
   using UnityEngine;
   using UnityEngine.InputSystem;

   namespace CrimsonBoard
   {
       public class PlayerMovementSystem : IGameSystem
       {
           private readonly GameContext _context;
           private readonly GridMovementSystem _gridMovement;
           private InputSystem_Actions _input;
           private float _cooldownRemaining;

           public PlayerMovementSystem(GameContext context, GridMovementSystem gridMovement)
           {
               _context = context;
               _gridMovement = gridMovement;
           }

           public void Initialize()
           {
               _input = new InputSystem_Actions();
               _input.Player.Enable();
               _cooldownRemaining = 0f;
           }

           public void Tick(float deltaTime)
           {
               _cooldownRemaining -= deltaTime;
               if (_cooldownRemaining > 0f) return;

               var raw = _input.Player.Move.ReadValue<Vector2>();
               if (raw.sqrMagnitude < 0.1f) return;

               var dir = RoundToGridDir(raw);
               if (dir == Vector2Int.zero) return;

               var result = _gridMovement.TryMove(_context.Player, dir);
               if (result == MoveResult.Moved)
               {
                   var timing = _context.Config.timing;
                   _cooldownRemaining = timing.beatDuration / Mathf.Max(1, _context.Config.player.movesPerBeat);

                   if (_context.Player.DirectionIndicator != null)
                       _context.Player.DirectionIndicator.rotation =
                           Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.y), Vector3.up);
               }
           }

           public void Dispose()
           {
               _input?.Player.Disable();
               _input?.Dispose();
           }

           // Converts a raw Vector2 into one of the 8 cardinal/diagonal grid directions.
           private static Vector2Int RoundToGridDir(Vector2 raw)
           {
               float angle = Mathf.Atan2(raw.y, raw.x) * Mathf.Rad2Deg;
               // Snap to nearest 45°
               int snapped = Mathf.RoundToInt(angle / 45f) * 45;
               float rad = snapped * Mathf.Deg2Rad;
               int x = Mathf.RoundToInt(Mathf.Cos(rad));
               int y = Mathf.RoundToInt(Mathf.Sin(rad));
               return new Vector2Int(x, y);
           }
       }
   }
   ```

## Implementation
**Status:** DONE
**Summary:** Создан `PlayerMovementSystem`: читает `Move` из InputSystem_Actions, снэппит в 8-directional dir через `RoundToGridDir`, применяет кулдаун `beatDuration/movesPerBeat`, при успешном ходе поворачивает `DirectionIndicator`.
