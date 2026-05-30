# Task 3: Create HudView and update GameplayState

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/UI/Views/HudView.cs`
- Modify: `CB-client/Assets/Scripts/States/GameplayState.cs`

**Commit message:** `13 Add HudView and update GameplayState`

### Steps

1. **Create `HudView.cs`** — extends `BaseView` in namespace `CrimsonBoard`:
   - SerializeFields:
     ```csharp
     [SerializeField] private TMP_Text _healthText;
     [SerializeField] private TMP_Text _weaponText;
     [SerializeField] private TMP_Text _ammoText;
     [SerializeField] private TMP_Text _timeText;
     [SerializeField] private TMP_Text _scoreText;
     [SerializeField] private Button _menuButton;
     ```
   - `public System.Action OnMenuRequested;` event.
   - Override `Show()`:
     ```csharp
     public override void Show()
     {
         base.Show();
         _menuButton.onClick.AddListener(HandleMenuClick);
     }
     ```
   - Override `Hide()`:
     ```csharp
     public override void Hide()
     {
         base.Hide();
         _menuButton.onClick.RemoveListener(HandleMenuClick);
     }
     ```
   - `private void HandleMenuClick() => OnMenuRequested?.Invoke();`
   - Override `Tick(float deltaTime)`: poll `GameContext.Instance` and update labels:
     ```csharp
     public override void Tick(float deltaTime)
     {
         var ctx = GameContext.Instance;
         if (ctx?.Player == null) return;

         _healthText.text = $"{ctx.Player.Health.CurrentHp:0} / {ctx.Player.Health.MaxHp:0}";
         _ammoText.text = "\u221e"; // ∞
         _scoreText.text = ctx.Stats.Score.ToString();

         var t = ctx.Stats.ElapsedBattleTime;
         _timeText.text = $"{(int)t / 60:00}:{(int)t % 60:00}";

         var weapon = ctx.Player.WeaponLocator.GetComponentInChildren<WeaponView>();
         _weaponText.text = weapon != null ? $"Weapon #{weapon.name}" : "—";
     }
     ```
   - Add `using TMPro; using UnityEngine; using UnityEngine.UI;`.

2. **Modify `GameplayState.cs`**:
   a. In `Enter()`: reset stats, subscribe menu event, show HudView:
      ```csharp
      _context.Stats.Reset();
      var hud = _context.UiRoot.GetView<HudView>();
      hud.OnMenuRequested = () => _fsm.RequestPause(new PauseState(_context, _fsm));
      _context.UiRoot.Show<HudView>();
      ```
   b. In `Exit()`: `_context.UiRoot.Hide<HudView>();`
   c. In constructor: hook score increment on enemy death:
      ```csharp
      _healthSystem.EnemyDeathCallback += _ => _context.Stats.AddScore(1);
      ```
      Place after `_healthSystem.EnemyDeathCallback += _enemySpawnSystem.OnEnemyDied;`.
   d. Override `Tick` to also advance stats timer:
      ```csharp
      public void Tick(float deltaTime)
      {
          _context.Stats.Tick(deltaTime);
          _systemRunner.Tick(deltaTime);
      }
      ```

## Implementation
<!-- Filled in Phase 3 -->
