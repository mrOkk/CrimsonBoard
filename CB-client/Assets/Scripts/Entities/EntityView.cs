using UnityEngine;

namespace CrimsonBoard
{
    public class EntityView : MonoBehaviour
    {
        private enum HopPhase { Idle, Windup, Hop, Knockback }

        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;

        public MeshFilter MeshFilter => _meshFilter;
        public Rigidbody Rigidbody => _rigidbody;
        public Collider Collider => _collider;

        public Vector2Int CurrentCell { get; set; }
        public bool IsMoving => _hopPhase != HopPhase.Idle;
        public bool IsKnockback => _knockbackActive;

        private HopPhase _hopPhase = HopPhase.Idle;
        private Vector3 _hopFrom;
        private Vector3 _hopTo;
        private Vector3 _windupOffset;
        private float _hopTimer;
        private HopConfig _hopConfig;

        private bool _knockbackActive;
        private Vector3 _knockbackVelocity;
        private Vector2Int _knockbackTargetCell;

        public void StartHop(Vector2Int dir, Vector3 from, Vector3 to, HopConfig config)
        {
            _hopConfig = config;
            _hopFrom = from;
            _hopTo = to;
            _windupOffset = new Vector3(-dir.x, 0f, -dir.y).normalized * config.windupAmplitude;
            _hopTimer = 0f;
            _hopPhase = HopPhase.Windup;
        }

        public void TickHop(float dt)
        {
            if (_hopPhase == HopPhase.Idle)
            {
                return;
            }

            _hopTimer += dt;

            if (_hopPhase == HopPhase.Windup)
            {
                float t = _hopConfig.windupDuration > 0f
                    ? Mathf.Clamp01(_hopTimer / _hopConfig.windupDuration)
                    : 1f;
                transform.position = _hopFrom + _windupOffset * Mathf.Sin(t * Mathf.PI);
                if (_hopTimer >= _hopConfig.windupDuration)
                {
                    _hopPhase = HopPhase.Hop;
                    _hopTimer = 0f;
                }
            }
            else if (_hopPhase == HopPhase.Hop)
            {
                float t = _hopConfig.hopDuration > 0f
                    ? Mathf.Clamp01(_hopTimer / _hopConfig.hopDuration)
                    : 1f;
                var flat = Vector3.Lerp(_hopFrom, _hopTo, t);
                transform.position = flat + new Vector3(0f, Mathf.Sin(t * Mathf.PI) * _hopConfig.hopHeight, 0f);
                if (_hopTimer >= _hopConfig.hopDuration)
                {
                    transform.position = _hopTo;
                    _hopPhase = HopPhase.Idle;
                }
            }
        }

        public void StartKnockback(Vector2Int fromCell, Vector2Int toCell, KnockbackConfig config, BoardConfig boardConfig)
        {
            _knockbackActive = true;
            _hopPhase = HopPhase.Knockback;
            _knockbackTargetCell = toCell;
            var tileMap = GameContext.Instance.TileMap;
            var fromPos = tileMap.CellToWorld(fromCell);
            var toPos = tileMap.CellToWorld(toCell);
            _knockbackVelocity = (toPos - fromPos).normalized * config.initialSpeed;
        }

        public void TickKnockback(float dt, Vector2Int? inputDir, KnockbackConfig config, BoardConfig boardConfig)
        {
            if (!_knockbackActive)
            {
                return;
            }

            if (inputDir.HasValue)
            {
                var inputWorld = new Vector3(inputDir.Value.x, 0f, inputDir.Value.y);
                _knockbackVelocity += inputWorld * (config.playerInfluence * dt);
            }

            transform.position += _knockbackVelocity * dt;

            _knockbackVelocity -= _knockbackVelocity.normalized * (config.friction * dt);
            if (_knockbackVelocity.magnitude < 0.1f)
            {
                var targetPos = GameContext.Instance.TileMap.CellToWorld(_knockbackTargetCell);
                transform.position = targetPos;
                _knockbackActive = false;
            }
        }

        public void CancelKnockback()
        {
            _knockbackActive = false;
        }
    }
}
