using UnityEngine;

namespace CrimsonBoard
{
    public class QueenMoveStrategy : IMoveStrategy
    {
        private static readonly Vector2Int[] AllDirs = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1),
        };

        public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
            => LinearStrategy.GetBestDirection(enemy, ctx, AllDirs, maxSteps: 6);
    }
}
