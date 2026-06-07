using UnityEngine;

namespace CrimsonBoard
{
    public class PawnMoveStrategy : IMoveStrategy
    {
        private static readonly Vector2Int[] Cardinals = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
        };

        public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
        {
            var playerCell = ctx.Player?.CurrentCell ?? Vector2Int.zero;
            Vector2Int? best = null;
            int bestDist = int.MaxValue;

            foreach (var dir in Cardinals)
            {
                var target = enemy.CurrentCell + dir;
                var occupant = ctx.TileMap.GetEntity(target);
                bool passable = occupant == null || occupant is PlayerView;
                if (!passable) continue;

                int dist = Mathf.Abs(target.x - playerCell.x) + Mathf.Abs(target.y - playerCell.y);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = dir;
                }
            }
            return best;
        }
    }
}
