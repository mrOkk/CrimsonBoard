using UnityEngine;

namespace CrimsonBoard
{
    public static class KnockbackResolver
    {
        // Fixed priority order for fallback cells (deterministic)
        private static readonly Vector2Int[] FallbackDirs = new[]
        {
            new Vector2Int(0, 1),   // North
            new Vector2Int(0, -1),  // South
            new Vector2Int(1, 0),   // East
            new Vector2Int(-1, 0),  // West
        };

        /// <summary>
        /// Returns the target cell to push the player to, or null if all adjacent cells are occupied.
        /// </summary>
        /// <param name="playerPos">Current player cell.</param>
        /// <param name="enemyDir">Direction the enemy was moving when it hit the player.</param>
        /// <param name="map">Current occupancy map.</param>
        public static Vector2Int? Resolve(Vector2Int playerPos, Vector2Int enemyDir, TileMap map)
        {
            // Primary: opposite of enemy movement direction
            var primary = enemyDir;
            if (!map.IsOccupied(enemyDir))
            {
                return primary;
            }

            // Fallback: fixed priority order, skip primary and current position
            for (var index = 0; index < FallbackDirs.Length; index++)
            {
                var dir = FallbackDirs[index];
                var candidate = playerPos + dir;

                if (candidate == primary)
                {
                    continue;
                }

                if (!map.IsOccupied(candidate))
                {
                    return candidate;
                }
            }

            return null; // All occupied — no knockback, only damage
        }
    }
}
