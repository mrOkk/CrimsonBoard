using UnityEngine;

namespace CrimsonBoard
{
    public class EnemyView : EntityView
    {
        [SerializeField] private HealthComponent _health;

        private EnemyConfig _config;

        public HealthComponent Health => _health;
        public EnemyConfig Config => _config;

        public void Setup(EnemyConfig config)
        {
            _config = config;
            MeshFilter.sharedMesh = config.mesh;
            _health.Init(config.health);
        }
    }
}
