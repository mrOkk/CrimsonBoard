# Task 2: Create PreBattleView and update TapToStartState

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/UI/Views/PreBattleView.cs`
- Modify: `CB-client/Assets/Scripts/States/TapToStartState.cs`

**Commit message:** `13 Add PreBattleView and update TapToStartState`

### Steps

1. **Create folder** `CB-client/Assets/Scripts/UI/Views/`.

2. **Create `PreBattleView.cs`** — extends `BaseView` in namespace `CrimsonBoard`:
   - `[SerializeField] private TMP_Text _label;` — label shown on screen (e.g. "Tap to Start")
   - `public System.Action OnPlayerInput;` — fired when any key or touch is detected
   - Override `Tick(float deltaTime)`: detect `Input.anyKeyDown` OR (`Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began`) → invoke `OnPlayerInput` and clear subscription to avoid double-fire:
     ```csharp
     public override void Tick(float deltaTime)
     {
         if (Input.anyKeyDown || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
             OnPlayerInput?.Invoke();
     }
     ```
   - Add `using TMPro;` and `using UnityEngine;`.

3. **Modify `TapToStartState.cs`**:
   a. Add `private readonly bool _autoStart;` field.
   b. Add `autoStart = false` optional param to constructor:
      ```csharp
      public TapToStartState(GameContext context, GameStateMachine fsm, bool autoStart = false)
      ```
   c. In `Enter()`: after existing init code, branch on `_autoStart`:
      - If `true`: immediately `_fsm.ChangeState(new GameplayState(_context, _fsm)); return;`
      - If `false`: show and subscribe:
        ```csharp
        var view = _context.UiRoot.GetView<PreBattleView>();
        view.OnPlayerInput = () => _fsm.ChangeState(new GameplayState(_context, _fsm));
        _context.UiRoot.Show<PreBattleView>();
        ```
   d. In `Exit()`: `_context.UiRoot.Hide<PreBattleView>();` (guard: only if not autoStart).
   e. Remove old `Tick()` input handling (the Space keydown and touch block) — input is now owned by PreBattleView.

## Implementation
<!-- Filled in Phase 3 -->
