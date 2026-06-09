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
            if (ctx?.Player == null)
            {
                return;
            }

            _healthText.SetText("{0:0} / {1:0}", ctx.Player.Health.CurrentHp, ctx.Player.Health.MaxHp);
            _ammoText.text = "\u221e";
            _scoreText.SetText("{0}", ctx.Stats.Score);
            var t = ctx.Stats.ElapsedBattleTime;
            _timeText.SetText("{0:00}:{1:00}", (int)t / 60, (int)t % 60);

            var weapon = ctx.Player.WeaponLocator.GetComponentInChildren<WeaponView>();
            _weaponText.text = weapon != null ? weapon.name : "\u2014";
        }

        private void HandleMenuClick() => OnMenuRequested?.Invoke();
    }
}
