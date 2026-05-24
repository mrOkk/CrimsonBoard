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
            _health.Init(config.health);
        }
    }
}
