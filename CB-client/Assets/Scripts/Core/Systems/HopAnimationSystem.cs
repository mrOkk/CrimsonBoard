namespace CrimsonBoard
{
    public class HopAnimationSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly EnemySpawnSystem _enemySpawn;

        public HopAnimationSystem(GameContext context, EnemySpawnSystem enemySpawn)
        {
            _context = context;
            _enemySpawn = enemySpawn;
        }

        public void Initialize() { }

        public void Tick(float deltaTime)
        {
            _context.Player?.TickHop(deltaTime);
            if (_enemySpawn == null) return;
            foreach (var enemy in _enemySpawn.ActiveEnemies)
                enemy.TickHop(deltaTime);
        }

        public void Dispose() { }
    }
}
