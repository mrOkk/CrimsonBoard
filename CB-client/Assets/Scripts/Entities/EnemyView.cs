using UnityEngine;

namespace CrimsonBoard
{
    public class EnemyView : EntityView
    {
        [SerializeField] private HealthComponent _health;
        [SerializeField] private DissolveEffect _dissolve;

        private EnemyConfig _config;

        public HealthComponent Health => _health;
        public EnemyConfig Config => _config;
        public DissolveEffect Dissolve => _dissolve;

        public void Setup(EnemyConfig config)
        {
            _config = config;
            MeshFilter.sharedMesh = config.mesh;
            _health.Init(config.health);
            _dissolve.ResetDissolve();
        }
    }
}
