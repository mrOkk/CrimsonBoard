using UnityEngine;

namespace CrimsonBoard
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileView : MonoBehaviour
    {
        private Rigidbody _rb;
        private float _speed;
        private float _damage;
        private int _pierceLeft;
        private float _lifetime;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Launch(Vector3 direction, float speed, float damage, int pierceCount, float range)
        {
            _speed = speed;
            _damage = damage;
            _pierceLeft = pierceCount;
            _lifetime = range / speed;

            _rb.linearVelocity = direction.normalized * speed;
        }

        private void Update()
        {
            if (_lifetime > 0f)
            {
                _lifetime -= Time.deltaTime;
                if (_lifetime <= 0f)
                    ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return;

            var enemyView = other.GetComponentInParent<EnemyView>();
            if (enemyView == null) return;

            var health = enemyView.Health;
            if (health != null && !health.IsDead)
            {
                health.TakeDamage(_damage);
                _pierceLeft--;

                if (_pierceLeft <= 0)
                {
                    ReturnToPool();
                }
            }
        }

        private void ReturnToPool()
        {
            _rb.linearVelocity = Vector3.zero;
            var ctx = GameContext.Instance;
            if (ctx != null && ctx.Pools != null)
                ctx.Pools.Projectiles.Return(this);
            else
                gameObject.SetActive(false);
        }
    }
}
