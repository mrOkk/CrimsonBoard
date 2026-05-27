using UnityEngine;

namespace CrimsonBoard
{
    /// <summary>
    /// Shared static helper for Rook, Tower, and Queen strategies.
    /// Walks up to <c>maxSteps</c> cells in each candidate direction;
    /// stops at the first blocked cell. Returns the single-step direction
    /// that brings the enemy closest to the player after sliding.
    /// </summary>
    internal static class LinearStrategy
    {
        internal static Vector2Int? GetBestDirection(
            EnemyView enemy, GameContext ctx, Vector2Int[] directions, int maxSteps)
        {
            var playerCell = ctx.Player?.CurrentCell ?? Vector2Int.zero;
            Vector2Int? bestDir = null;
            int bestDist = int.MaxValue;

            foreach (var dir in directions)
            {
                Vector2Int reached = enemy.CurrentCell;
                for (int step = 1; step <= maxSteps; step++)
                {
                    var next = enemy.CurrentCell + dir * step;
                    var occupant = ctx.OccupancyMap.GetEntity(next);
                    if (occupant != null && !(occupant is PlayerView)) break; // blocked by non-player
                    reached = next;
                    if (occupant is PlayerView) break; // stop at player cell
                }
                if (reached == enemy.CurrentCell) continue; // no movement possible this way

                int dist = Mathf.Abs(reached.x - playerCell.x) + Mathf.Abs(reached.y - playerCell.y);
                if (dist < bestDist) { bestDist = dist; bestDir = dir; }
            }
            return bestDir;
        }
    }
}
