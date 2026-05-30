using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrimsonBoard
{
    public class PostBattleView : BaseView
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _restartButton;

        public System.Action OnRestart;

        public override void Show()
        {
            base.Show();
            var stats = GameContext.Instance.Stats;
            _scoreText.text = stats.Score.ToString();
            var t = stats.ElapsedBattleTime;
            _timeText.text = $"{(int)t / 60:00}:{(int)t % 60:00}";
            _restartButton.onClick.AddListener(HandleRestart);
        }

        public override void Hide()
        {
            base.Hide();
            _restartButton.onClick.RemoveListener(HandleRestart);
        }

        private void HandleRestart() => OnRestart?.Invoke();
    }
}
