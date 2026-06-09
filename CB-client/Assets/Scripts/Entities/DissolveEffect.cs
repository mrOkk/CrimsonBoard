using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CrimsonBoard
{
    public class DissolveEffect : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.6f;
        private bool _isDissolving;

        /// <summary>Animates dissolve from 0 → 1 over <see cref="_duration"/> seconds.</summary>
        public void Play(Action onComplete = null)
        {
            if (_isDissolving)
            {
                return;
            }

            _isDissolving = true;
            DissolveRoutine(onComplete).Forget();
        }

        /// <summary>Stops any running animation and resets to fully visible.</summary>
        public void ResetDissolve()
        {
            if (!_isDissolving)
            {
                return;
            }

            _isDissolving = false;
        }

        private async UniTask DissolveRoutine(Action onComplete)
        {
            await UniTask.Delay((int)(_duration * 1000));
            onComplete?.Invoke();
            _isDissolving = false;
        }
    }
}
