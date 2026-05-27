# Task 2: EntityView hop animation + GridMovementSystem

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Entities/EntityView.cs`
- Modify: `CB-client/Assets/Scripts/Core/Systems/GridMovementSystem.cs`

**Commit message:** `09 Add StartHop/TickHop to EntityView; GridMovementSystem triggers hop`

### Steps

1. **Add hop state fields to `EntityView.cs`** — insert private fields and a `HopPhase` enum before the existing public properties:
   ```csharp
   private enum HopPhase { Idle, Windup, Hop }
   private HopPhase _hopPhase = HopPhase.Idle;
   private Vector3 _hopFrom;
   private Vector3 _hopTo;
   private Vector3 _windupOffset;
   private float _hopTimer;
   private HopConfig _hopConfig;
   ```

2. **Add `StartHop` method to `EntityView.cs`** — public method to kick off the animation. Called by `GridMovementSystem` after logical state update:
   ```csharp
   public void StartHop(Vector2Int dir, Vector3 from, Vector3 to, HopConfig config)
   {
       _hopConfig = config;
       _hopFrom = from;
       _hopTo = to;
       _windupOffset = new Vector3(-dir.x, 0f, -dir.y).normalized * config.windupAmplitude;
       _hopTimer = 0f;
       _hopPhase = HopPhase.Windup;
   }
   ```

3. **Add `TickHop` method to `EntityView.cs`** — called every frame by `HopAnimationSystem`. Two-phase: windup then arc:
   ```csharp
   public void TickHop(float dt)
   {
       if (_hopPhase == HopPhase.Idle) return;

       _hopTimer += dt;

       if (_hopPhase == HopPhase.Windup)
       {
           float t = _hopConfig.windupDuration > 0f
               ? Mathf.Clamp01(_hopTimer / _hopConfig.windupDuration)
               : 1f;
           transform.position = _hopFrom + _windupOffset * Mathf.Sin(t * Mathf.PI);
           if (_hopTimer >= _hopConfig.windupDuration)
           {
               _hopPhase = HopPhase.Hop;
               _hopTimer = 0f;
           }
       }
       else if (_hopPhase == HopPhase.Hop)
       {
           float t = _hopConfig.hopDuration > 0f
               ? Mathf.Clamp01(_hopTimer / _hopConfig.hopDuration)
               : 1f;
           var flat = Vector3.Lerp(_hopFrom, _hopTo, t);
           transform.position = flat + new Vector3(0f, Mathf.Sin(t * Mathf.PI) * _hopConfig.hopHeight, 0f);
           if (_hopTimer >= _hopConfig.hopDuration)
           {
               transform.position = _hopTo;
               _hopPhase = HopPhase.Idle;
           }
       }
   }
   ```

4. **Update `GridMovementSystem.TryMove`** — replace the line that snaps `entity.transform.position` with a `StartHop` call. Capture `fromPos` before updating `CurrentCell`, compute `toPos` from the target cell:
   ```csharp
   // Before the logical move (add this line):
   var fromPos = entity.transform.position;

   // Keep existing logical updates:
   _context.OccupancyMap.Unregister(entity.CurrentCell);
   entity.CurrentCell = targetCell;
   _context.OccupancyMap.Register(targetCell, entity);

   // Replace the snap line with:
   var toPos = ChunkCoordConverter.TileToWorld(targetCell, _context.Config.board);
   entity.StartHop(dir, fromPos, toPos, _context.Config.hop);
   ```
   Also add `using UnityEngine;` if not already present (it already is).

## Implementation
**Status:** DONE
**Summary:** Added `HopPhase` enum, hop state fields, `StartHop`, and `TickHop` to `EntityView`. Updated `GridMovementSystem.TryMove` to capture `fromPos`, keep `OccupancyMap.Register` after `CurrentCell` update, and call `entity.StartHop(...)` with world positions instead of snapping.
