# Task 5: Create PostBattleView and update GameOverState

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/UI/Views/PostBattleView.cs`
- Modify: `CB-client/Assets/Scripts/States/GameOverState.cs`

**Commit message:** `13 Add PostBattleView and update GameOverState`

### Steps

1. **Create `PostBattleView.cs`** — extends `BaseView` in namespace `CrimsonBoard`:
   - SerializeFields:
     ```csharp
     [SerializeField] private TMP_Text _scoreText;
     [SerializeField] private TMP_Text _timeText;
     [SerializeField] private Button _restartButton;
     ```
   - Event: `public System.Action OnRestart;`
   - Override `Show()`: populate texts from `GameContext.Instance.Stats`, then subscribe button:
     ```csharp
     public override void Show()
     {
         base.Show();
         var stats = GameContext.Instance.Stats;
         _scoreText.text = stats.Score.ToString();
         var t = stats.ElapsedBattleTime;
         _timeText.text = $"{(int)t / 60:00}:{(int)t % 60:00}";
         _restartButton.onClick.AddListener(HandleRestart);
     }
     ```
   - Override `Hide()`:
     ```csharp
     public override void Hide()
     {
         base.Hide();
         _restartButton.onClick.RemoveListener(HandleRestart);
     }
     ```
   - `private void HandleRestart() => OnRestart?.Invoke();`
   - Add `using TMPro; using UnityEngine; using UnityEngine.UI;`.

2. **Modify `GameOverState.cs`**:
   a. In `Enter()`: subscribe and show PostBattleView:
      ```csharp
      public void Enter()
      {
          var view = _context.UiRoot.GetView<PostBattleView>();
          view.OnRestart = () => _fsm.ChangeState(new TapToStartState(_context, _fsm, autoStart: true));
          _context.UiRoot.Show<PostBattleView>();
      }
      ```
   b. In `Exit()`: `_context.UiRoot.Hide<PostBattleView>();`
   c. Remove `// TODO` comment from `Tick()`.

## Implementation
<!-- Filled in Phase 3 -->
