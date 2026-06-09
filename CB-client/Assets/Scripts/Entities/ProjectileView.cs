using UnityEngine;

namespace CrimsonBoard
{
    public class ProjectileView : MonoBehaviour
    {
        private static readonly LayerMask EnemyLayerMask = 1 << 7;

        private Vector3 _direction;
        private float _speed;
        private float _damage;
        private int _pierceLeft;
        private float _remainingDistance;
        private float _radius;

        public void Launch(Vector3 direction, float speed, float damage, int pierceCount, float range, float radius)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _pierceLeft = pierceCount;
            _remainingDistance = range;
            _radius = radius;
        }

        private void Update()
        {
            if (_remainingDistance <= 0f)
            {
                ReturnToPool();
                return;
            }

            float moveDistance = _speed * Time.deltaTime;
            float actualMove = Mathf.Min(moveDistance, _remainingDistance);

            Vector3 origin = transform.position;

            if (Physics.SphereCast(origin, _radius, _direction, out RaycastHit hit, actualMove, EnemyLayerMask, QueryTriggerInteraction.Ignore))
            {
                float traveled = hit.distance;
                transform.position = origin + _direction * traveled;
                _remainingDistance -= traveled;

                if (ProcessHit(hit.collider))
                    return;
            }
            else
            {
                transform.position = origin + _direction * actualMove;
                _remainingDistance -= actualMove;
            }
        }

        private bool ProcessHit(Collider hitCollider)
        {
            if (hitCollider == null) return false;

            var enemyView = hitCollider.GetComponentInParent<EnemyView>();
            if (enemyView == null) return false;

            var health = enemyView.Health;
            if (health != null && !health.IsDead)
            {
                health.TakeDamage(_damage);
                _pierceLeft--;

                var ctx = GameContext.Instance;
                if (ctx != null && ctx.HitEmitter != null)
                    ctx.HitEmitter.Emit(transform.position);

                if (_pierceLeft <= 0)
                {
                    ReturnToPool();
                    return true;
                }
            }

            return false;
        }

        private void ReturnToPool()
        {
            var ctx = GameContext.Instance;
            if (ctx != null && ctx.Pools != null)
                ctx.Pools.Projectiles.Return(this);
            else
                gameObject.SetActive(false);
        }
    }
}
