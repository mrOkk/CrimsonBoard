using UnityEngine;

namespace CrimsonBoard
{
    public class HealthSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;

        public HealthSystem(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
        }

        public void Initialize()
        {
            _context.Player.Health.Init(_context.Config.player.health);

            // TODO: subscribe to enemy-entered-cell events when enemy movement is implemented
        }

        public void Tick(float deltaTime) { }

        public void Dispose()
        {
            // TODO: unsubscribe from events
        }

        /// <summary>
        /// Call this when an enemy enters the player's cell.
        /// </summary>
        /// <param name="enemy">The attacking enemy.</param>
        /// <param name="enemyDir">Grid direction the enemy was moving.</param>
        /// <param name="playerCell">Current cell of the player.</param>
        public void ApplyDamageToPlayer(EnemyView enemy, Vector2Int enemyDir, Vector2Int playerCell)
        {
            _context.Player.Health.TakeDamage(enemy.Config.damage);

            if (_context.Player.Health.IsDead)
            {
                _fsm.ChangeState(new GameOverState(_context, _fsm));
                return;
            }

            var targetCell = KnockbackResolver.Resolve(playerCell, enemyDir, _context.OccupancyMap);
            if (targetCell.HasValue)
            {
                _context.OccupancyMap.Unregister(playerCell);
                // TODO: move player transform to world position of targetCell (needs ChunkCoordConverter + tileSize)
                _context.OccupancyMap.Register(targetCell.Value, _context.Player);
            }
            // else: all cells occupied — only damage applied, player stays
        }

        /// <summary>
        /// Call this when an enemy's HP reaches zero.
        /// </summary>
        public void OnEnemyDeath(EnemyView enemy, Vector2Int enemyCell)
        {
            _context.OccupancyMap.Unregister(enemyCell);
            _context.Pools.Enemies.Return(enemy);
        }
    }
}
