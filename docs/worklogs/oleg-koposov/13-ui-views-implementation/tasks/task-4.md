# Task 4: Create MenuView and update PauseState

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/UI/Views/MenuView.cs`
- Modify: `CB-client/Assets/Scripts/States/PauseState.cs`

**Commit message:** `13 Add MenuView and update PauseState`

### Steps

1. **Create `MenuView.cs`** — extends `BaseView` in namespace `CrimsonBoard`:
   - SerializeFields:
     ```csharp
     [SerializeField] private Button _continueButton;
     [SerializeField] private Button _restartButton;
     [SerializeField] private Toggle _audioToggle;
     [SerializeField] private Slider _volumeSlider;
     ```
   - Events: `public System.Action OnContinue;`, `public System.Action OnRestart;`
   - Override `Show()`:
     ```csharp
     public override void Show()
     {
         base.Show();
         _audioToggle.isOn = !AudioListener.pause;
         _volumeSlider.value = AudioListener.volume;
         _continueButton.onClick.AddListener(HandleContinue);
         _restartButton.onClick.AddListener(HandleRestart);
         _audioToggle.onValueChanged.AddListener(HandleAudioToggle);
         _volumeSlider.onValueChanged.AddListener(HandleVolumeSlider);
     }
     ```
   - Override `Hide()`:
     ```csharp
     public override void Hide()
     {
         base.Hide();
         _continueButton.onClick.RemoveListener(HandleContinue);
         _restartButton.onClick.RemoveListener(HandleRestart);
         _audioToggle.onValueChanged.RemoveListener(HandleAudioToggle);
         _volumeSlider.onValueChanged.RemoveListener(HandleVolumeSlider);
     }
     ```
   - Handlers:
     ```csharp
     private void HandleContinue() => OnContinue?.Invoke();
     private void HandleRestart() => OnRestart?.Invoke();
     private void HandleAudioToggle(bool on) => AudioListener.pause = !on;
     private void HandleVolumeSlider(float v) => AudioListener.volume = v;
     ```
   - Add `using UnityEngine; using UnityEngine.UI;`.

2. **Modify `PauseState.cs`**:
   a. In `Enter()`: subscribe to menu events and show MenuView:
      ```csharp
      public void Enter()
      {
          var menu = _context.UiRoot.GetView<MenuView>();
          menu.OnContinue = () => _fsm.ResumePreviousState();
          menu.OnRestart  = () => _fsm.ChangeState(new TapToStartState(_context, _fsm, autoStart: true));
          _context.UiRoot.Show<MenuView>();
      }
      ```
   b. In `Exit()`: `_context.UiRoot.Hide<MenuView>();`
   c. Remove existing `// TODO` comment from `Tick()`.

## Implementation
<!-- Filled in Phase 3 -->
