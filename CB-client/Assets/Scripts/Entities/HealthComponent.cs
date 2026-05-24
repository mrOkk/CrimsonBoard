using UnityEngine;

namespace CrimsonBoard
{
    public class HealthComponent : MonoBehaviour
    {
        private float _maxHp;
        private float _currentHp;

        public float CurrentHp => _currentHp;
        public float MaxHp => _maxHp;
        public bool IsDead => _currentHp <= 0f;

        public System.Action OnDeath;

        public void Init(float maxHp)
        {
            _maxHp = maxHp;
            _currentHp = maxHp;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;
            _currentHp = Mathf.Max(0f, _currentHp - amount);
            if (IsDead)
                OnDeath?.Invoke();
        }

        public void Heal(float amount)
        {
            _currentHp = Mathf.Min(_maxHp, _currentHp + amount);
        }
    }
}
