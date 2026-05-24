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
    }
}
