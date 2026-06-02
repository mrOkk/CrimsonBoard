using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CrimsonBoard
{
    public class DissolveEffect : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _duration = 0.6f;

        private static readonly int DissolveAmountId = Shader.PropertyToID("_DissolveAmount");

        private MaterialPropertyBlock _block;
        private CancellationTokenSource _cts;

        /// <summary>Animates dissolve from 0 → 1 over <see cref="_duration"/> seconds.</summary>
        public void Play(Action onComplete = null)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
            DissolveRoutine(onComplete, _cts.Token).Forget();
        }

        /// <summary>Stops any running animation and resets to fully visible.</summary>
        public void ResetDissolve()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            SetAmount(0f);
        }

        private void SetAmount(float value)
        {
            _block ??= new();
            _block.SetFloat(DissolveAmountId, value);
            _renderer.SetPropertyBlock(_block);
        }

        private async UniTask DissolveRoutine(Action onComplete, CancellationToken ct)
        {
            float elapsed = 0f;
            while (elapsed < _duration)
            {
                if (ct.IsCancellationRequested)
                    return;
                elapsed += Time.deltaTime;
                SetAmount(Mathf.Clamp01(elapsed / _duration));
                await UniTask.Yield(cancellationToken: ct);
            }
            SetAmount(1f);
            onComplete?.Invoke();
        }
    }
}
