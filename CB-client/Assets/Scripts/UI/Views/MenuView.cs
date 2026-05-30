using UnityEngine;
using UnityEngine.UI;

namespace CrimsonBoard
{
    public class MenuView : BaseView
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Toggle _audioToggle;
        [SerializeField] private Slider _volumeSlider;

        public System.Action OnContinue;
        public System.Action OnRestart;

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

        public override void Hide()
        {
            base.Hide();
            _continueButton.onClick.RemoveListener(HandleContinue);
            _restartButton.onClick.RemoveListener(HandleRestart);
            _audioToggle.onValueChanged.RemoveListener(HandleAudioToggle);
            _volumeSlider.onValueChanged.RemoveListener(HandleVolumeSlider);
        }

        private void HandleContinue() => OnContinue?.Invoke();
        private void HandleRestart() => OnRestart?.Invoke();
        private void HandleAudioToggle(bool on) => AudioListener.pause = !on;
        private void HandleVolumeSlider(float v) => AudioListener.volume = v;
    }
}
