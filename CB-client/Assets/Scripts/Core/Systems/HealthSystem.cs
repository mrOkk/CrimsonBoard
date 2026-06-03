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

        /// <summary>Called when an enemy dies, before it is returned to the pool.</summary>
        public System.Action<EnemyView> EnemyDeathCallback;

        /// <summary>Invoked after enemy death with the WeaponView that was dropped (if any).</summary>
        public System.Action<WeaponView> WeaponDropped;

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
                _context.Player.CurrentCell = targetCell.Value;
                _context.Player.transform.position = ChunkCoordConverter.TileToWorld(targetCell.Value, _context.Config.board);
                _context.OccupancyMap.Register(targetCell.Value, _context.Player);
            }
            // else: all cells occupied — only damage applied, player stays
        }

        /// <summary>
        /// Call this when an enemy's HP reaches zero.
        /// </summary>
        public void OnEnemyDeath(EnemyView enemy, Vector2Int enemyCell)
        {
            EnemyDeathCallback?.Invoke(enemy);
            DissolveService.DissolveAndReturn(enemy, enemyCell, _context.OccupancyMap, _context.Pools);
            TryDropWeapon(enemyCell);
        }

        private void TryDropWeapon(Vector2Int enemyCell)
        {
            var weapons = _context.Config.weapons;
            float totalWeight = 0f;
            foreach (var w in weapons)
                if (w.dropChance > 0f) totalWeight += w.dropChance;
            if (totalWeight <= 0f) return;

            float roll = (float)(_context.SharedRandom.NextDouble() * totalWeight);
            float acc = 0f;
            WeaponConfig chosen = null;
            foreach (var w in weapons)
            {
                if (w.dropChance <= 0f) continue;
                acc += w.dropChance;
                if (roll < acc) { chosen = w; break; }
            }
            if (chosen == null) return;

            var pool = _context.Pools.GetWeaponPool(chosen.id);
            if (pool == null) return;

            var weaponView = pool.Get();
            var worldPos = ChunkCoordConverter.TileToWorld(enemyCell, _context.Config.board);
            weaponView.SetDroppedMode(worldPos);
            WeaponDropped?.Invoke(weaponView);
        }
    }
}
