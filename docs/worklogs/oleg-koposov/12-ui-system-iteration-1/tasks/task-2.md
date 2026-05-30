# Task 2: Integrate UiRoot into GameContext and EntryPoint

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Modify: `CB-client/Assets/Scripts/Core/EntryPoint.cs`

**Commit message:** `12 Wire UiRoot into GameContext and EntryPoint`

### Steps

1. **Modify `GameContext.cs`** — add a public property after the existing `GameFieldSystem` property:
   ```csharp
   public UiRoot UiRoot { get; set; }
   ```
   No constructor changes needed; `EntryPoint` sets it after creation.

2. **Modify `EntryPoint.cs`** — three changes:
   a. Add `[SerializeField] private UiRoot _uiRoot;` field alongside `_camera`.
   b. In `Awake`, after `_fsm.ChangeState(new InitState(...))`, initialize and assign:
      ```csharp
      _uiRoot?.Init();
      context.UiRoot = _uiRoot;
      ```
   c. In `Update`, after `_fsm.Tick(Time.deltaTime)`, add:
      ```csharp
      _uiRoot?.Tick(Time.deltaTime);
      ```
   Precedent: `_fsm.Tick(Time.deltaTime)` already follows this update-loop pattern in `EntryPoint.Update`.

## Implementation
<!-- Filled in Phase 3 -->
