using UnityEngine;

namespace CrimsonBoard
{
    public class GridMovementSystem : IGameSystem
    {
        private readonly GameContext _context;

        public HealthSystem HealthSystem { get; set; }

        public GridMovementSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize() { }
        public void Tick(float deltaTime) { }
        public void Dispose() { }

        /// <summary>
        /// Attempts to move <paramref name="entity"/> one step in <paramref name="dir"/>.
        /// Updates OccupancyMap, transform, and CurrentCell on success.
        /// Triggers HealthSystem when an enemy steps into the player's cell.
        /// </summary>
        public MoveResult TryMove(EntityView entity, Vector2Int dir)
        {
            var targetCell = entity.CurrentCell + dir;
            var occupant = _context.OccupancyMap.GetEntity(targetCell);

            if (occupant != null)
            {
                // Combat: enemy steps into player cell
                if (entity is EnemyView enemy && occupant is PlayerView)
                {
                    HealthSystem?.ApplyDamageToPlayer(enemy, dir, occupant.CurrentCell);
                    return MoveResult.Combat;
                }
                return MoveResult.Blocked;
            }

            // Move
            _context.OccupancyMap.Unregister(entity.CurrentCell);
            entity.CurrentCell = targetCell;
            entity.transform.position = ChunkCoordConverter.TileToWorld(targetCell, _context.Config.board);
            _context.OccupancyMap.Register(targetCell, entity);
            return MoveResult.Moved;
        }
    }
}
