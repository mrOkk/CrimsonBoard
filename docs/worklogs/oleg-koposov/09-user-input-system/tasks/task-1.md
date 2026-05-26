# Task 1: Configs and InputState scaffolding

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Configs/HopConfig.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/PlayerConfig.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/GameConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/InputState.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`

**Commit message:** `09 Add HopConfig, InputState, and config fields`

### Steps

1. **Create `HopConfig.cs`** in `Core/Configs/`. Following the `[Serializable]` pattern of `TimingConfig` and `PlayerConfig`:
   ```csharp
   using System;
   using UnityEngine;

   namespace CrimsonBoard
   {
       [Serializable]
       public class HopConfig
       {
           public float hopHeight = 0.5f;
           public float hopDuration = 0.15f;
           public float windupAmplitude = 0.1f;
           public float windupDuration = 0.05f;
       }
   }
   ```

2. **Add fields to `PlayerConfig.cs`** — append two new public fields after the existing ones:
   ```csharp
   public float movementInputDelay = 0.1f;
   public float inputBufferWindow = 0.15f;
   ```

3. **Add `hop` field to `GameConfig.cs`** — append after `public PrefabsConfig prefabs;`:
   ```csharp
   public HopConfig hop;
   ```

4. **Create `InputState.cs`** in `Core/`. Plain class (no `[Serializable]` needed — lives only in memory at runtime):
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class InputState
       {
           public Vector2Int? MoveCommand { get; set; }
           public bool ShootCommandBuffered { get; set; }
       }
   }
   ```

5. **Add `InputState` property to `GameContext.cs`** — add after the `GameFieldSystem` property declaration and initialise it in the constructor body (or as an auto-initialised property):
   ```csharp
   public InputState InputState { get; } = new InputState();
   ```

## Implementation
<!-- Filled in Phase 3 -->
