using System;
using System.Collections;
using UnityEngine;

namespace CrimsonBoard
{
    public class DissolveEffect : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _duration = 0.6f;

        private static readonly int DissolveAmountId = Shader.PropertyToID("_DissolveAmount");

        private readonly MaterialPropertyBlock _block = new();
        private Coroutine _coroutine;

        /// <summary>Animates dissolve from 0 → 1 over <see cref="_duration"/> seconds.</summary>
        public void Play(Action onComplete = null)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(DissolveRoutine(onComplete));
        }

        /// <summary>Stops any running animation and resets to fully visible.</summary>
        public void ResetDissolve()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            SetAmount(0f);
        }

        private void SetAmount(float value)
        {
            _block.SetFloat(DissolveAmountId, value);
            _renderer.SetPropertyBlock(_block);
        }

        private IEnumerator DissolveRoutine(Action onComplete)
        {
            float elapsed = 0f;
            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                SetAmount(Mathf.Clamp01(elapsed / _duration));
                yield return null;
            }
            SetAmount(1f);
            _coroutine = null;
            onComplete?.Invoke();
        }
    }
}
