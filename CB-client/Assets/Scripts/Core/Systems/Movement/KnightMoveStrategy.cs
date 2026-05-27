using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class KnightMoveStrategy : IMoveStrategy
    {
        private static readonly Vector2Int[] LMoves = {
            new Vector2Int(1, 2),  new Vector2Int(-1, 2),
            new Vector2Int(1, -2), new Vector2Int(-1, -2),
            new Vector2Int(2, 1),  new Vector2Int(-2, 1),
            new Vector2Int(2, -1), new Vector2Int(-2, -1),
        };

        public Vector2Int? GetMoveDirection(EnemyView enemy, GameContext ctx, System.Random rng)
        {
            var playerCell = ctx.Player?.CurrentCell ?? Vector2Int.zero;
            int myRank = enemy.Config.rank;

            // Early landing: if player is on any intermediate cell, step toward it
            foreach (var lMove in LMoves)
            {
                var intermediates = GetIntermediateCells(enemy.CurrentCell, lMove);
                foreach (var iCell in intermediates)
                {
                    if (iCell == playerCell)
                        return GetStepToward(enemy.CurrentCell, iCell);
                }
            }

            // Normal target selection: pick L-jump closest to player
            Vector2Int? best = null;
            int bestDist = int.MaxValue;

            foreach (var lMove in LMoves)
            {
                var target = enemy.CurrentCell + lMove;
                var occupant = ctx.OccupancyMap.GetEntity(target);

                if (occupant == null || occupant is PlayerView)
                {
                    // Valid target — empty or player
                }
                else if (occupant is EnemyView otherEnemy && otherEnemy.Config.rank < myRank)
                {
                    // Can crush lower-rank enemy
                }
                else
                {
                    // Equal or higher rank — try fallback to last free intermediate cell
                    var fallback = GetLastFreeIntermediate(enemy.CurrentCell, lMove, ctx.OccupancyMap);
                    if (fallback.HasValue)
                    {
                        int fd = Manhattan(fallback.Value, playerCell);
                        if (fd < bestDist)
                        {
                            bestDist = fd;
                            best = GetStepToward(enemy.CurrentCell, fallback.Value);
                        }
                    }
                    continue;
                }

                int dist = Manhattan(target, playerCell);
                if (dist < bestDist) { bestDist = dist; best = lMove; }
            }
            return best;
        }

        private static List<Vector2Int> GetIntermediateCells(Vector2Int from, Vector2Int lMove)
        {
            int ax = Mathf.Abs(lMove.x), ay = Mathf.Abs(lMove.y);
            int sx = System.Math.Sign(lMove.x), sy = System.Math.Sign(lMove.y);
            var cells = new List<Vector2Int>();

            if (ax == 2) // move 2 in x, 1 in y
            {
                cells.Add(from + new Vector2Int(sx, 0));
                cells.Add(from + new Vector2Int(2 * sx, 0));
                cells.Add(from + new Vector2Int(2 * sx, sy));
            }
            else // move 1 in x, 2 in y
            {
                cells.Add(from + new Vector2Int(0, sy));
                cells.Add(from + new Vector2Int(0, 2 * sy));
                cells.Add(from + new Vector2Int(sx, 2 * sy));
            }
            return cells;
        }

        private static Vector2Int? GetLastFreeIntermediate(Vector2Int from, Vector2Int lMove, OccupancyMap map)
        {
            var cells = GetIntermediateCells(from, lMove);
            Vector2Int? last = null;
            foreach (var c in cells)
                if (!map.IsOccupied(c)) last = c;
            return last;
        }

        private static Vector2Int GetStepToward(Vector2Int from, Vector2Int target)
        {
            var delta = target - from;
            return new Vector2Int(System.Math.Sign(delta.x), System.Math.Sign(delta.y));
        }

        private static int Manhattan(Vector2Int a, Vector2Int b)
            => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
