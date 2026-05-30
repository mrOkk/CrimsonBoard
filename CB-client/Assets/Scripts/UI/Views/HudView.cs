using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrimsonBoard
{
    public class HudView : BaseView
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _weaponText;
        [SerializeField] private TMP_Text _ammoText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Button _menuButton;

        public System.Action OnMenuRequested;

        public override void Show()
        {
            base.Show();
            _menuButton.onClick.AddListener(HandleMenuClick);
        }

        public override void Hide()
        {
            base.Hide();
            _menuButton.onClick.RemoveListener(HandleMenuClick);
        }

        public override void Tick(float deltaTime)
        {
            var ctx = GameContext.Instance;
            if (ctx?.Player == null) return;

            _healthText.text = $"{ctx.Player.Health.CurrentHp:0} / {ctx.Player.Health.MaxHp:0}";
            _ammoText.text = "\u221e";
            _scoreText.text = ctx.Stats.Score.ToString();

            var t = ctx.Stats.ElapsedBattleTime;
            _timeText.text = $"{(int)t / 60:00}:{(int)t % 60:00}";

            var weapon = ctx.Player.WeaponLocator.GetComponentInChildren<WeaponView>();
            _weaponText.text = weapon != null ? weapon.name : "\u2014";
        }

        private void HandleMenuClick() => OnMenuRequested?.Invoke();
    }
}
