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
            var tileData = _context.TileMap.GetTile(targetCell);

            if (tileData.IsOccupied)
            {
                if (entity is EnemyView enemy && tileData.Occupant is PlayerView)
                {
                    HealthSystem?.ApplyDamageToPlayer(enemy, dir, tileData.Occupant.CurrentCell);
                    return MoveResult.Combat;
                }
                return MoveResult.Blocked;
            }

            var fromPos = entity.transform.position;
            _context.TileMap.UnregisterEntity(entity.CurrentCell);
            entity.CurrentCell = targetCell;
            _context.TileMap.RegisterEntity(targetCell, entity);
            var toPos = ChunkCoordConverter.TileToWorld(targetCell, _context.Config.board);
            entity.StartHop(dir, fromPos, toPos, _context.Config.hop);
            return MoveResult.Moved;
        }
    }
}
