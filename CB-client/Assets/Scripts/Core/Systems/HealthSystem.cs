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
            // return; // TODO: re-enable when enemy movement is implemented
            _context.Player.Health.TakeDamage(enemy.Config.damage);

            if (_context.Player.Health.IsDead)
            {
                _fsm.ChangeState(new GameOverState(_context, _fsm));
                return;
            }

            var targetCell = KnockbackResolver.Resolve(playerCell, enemyDir, _context.TileMap);
            if (targetCell.HasValue)
            {
                _context.TileMap.UnregisterEntity(playerCell);
                _context.Player.CurrentCell = targetCell.Value;
                _context.TileMap.RegisterEntity(targetCell.Value, _context.Player);
                _context.Player.StartKnockback(playerCell, targetCell.Value, _context.Config.knockback, _context.Config.board);
            }
        }

        /// <summary>
        /// Call this when an enemy's HP reaches zero.
        /// </summary>
        public void OnEnemyDeath(EnemyView enemy, Vector2Int enemyCell)
        {
            EnemyDeathCallback?.Invoke(enemy);
            DissolveService.DissolveAndReturn(enemy, enemyCell, _context.TileMap, _context.Pools);
            TryDropWeapon(enemyCell);
        }

        private void TryDropWeapon(Vector2Int enemyCell)
        {
            var weapons = _context.Config.weapons;
            var totalWeight = 0f;

            for (var index = 0; index < weapons.Length; index++)
            {
                var w = weapons[index];
                if (w.dropChance > 0f) totalWeight += w.dropChance;
            }

            if (totalWeight <= 0f)
            {
                return;
            }

            var roll = (float)(_context.SharedRandom.NextDouble() * totalWeight);
            var acc = 0f;

            for (var index = 0; index < weapons.Length; index++)
            {
                var w = weapons[index];

                if (w.dropChance <= 0f)
                {
                    continue;
                }

                acc += w.dropChance;

                if (roll < acc)
                {
                    break;
                }
            }
        }
    }
}
