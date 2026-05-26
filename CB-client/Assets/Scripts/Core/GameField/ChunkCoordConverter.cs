using UnityEngine;

namespace CrimsonBoard
{
    public static class ChunkCoordConverter
    {
        public static Vector2Int WorldToChunk(Vector3 worldPos, BoardConfig config)
        {
            float chunkWorldSizeX = config.chunkSize * config.tileSize.x;
            float chunkWorldSizeZ = config.chunkSize * config.tileSize.y;
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / chunkWorldSizeX),
                Mathf.FloorToInt(worldPos.z / chunkWorldSizeZ)
            );
        }

        public static Vector3 ChunkToWorld(Vector2Int coord, BoardConfig config)
        {
            float chunkWorldSizeX = config.chunkSize * config.tileSize.x;
            float chunkWorldSizeZ = config.chunkSize * config.tileSize.y;
            return new Vector3(coord.x * chunkWorldSizeX, 0f, coord.y * chunkWorldSizeZ);
        }

        /// <summary>Converts a world position to tile grid coordinates.</summary>
        public static Vector2Int WorldToTile(Vector3 worldPos, BoardConfig config)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / config.tileSize.x),
                Mathf.FloorToInt(worldPos.z / config.tileSize.y)
            );
        }

        /// <summary>Returns the world-space centre of a tile cell.</summary>
        public static Vector3 TileToWorld(Vector2Int cell, BoardConfig config)
        {
            return new Vector3(
                (cell.x + 0.5f) * config.tileSize.x,
                0f,
                (cell.y + 0.5f) * config.tileSize.y
            );
        }
    }
}
