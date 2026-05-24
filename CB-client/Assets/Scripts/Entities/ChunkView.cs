using UnityEngine;

namespace CrimsonBoard
{
    public class ChunkView : MonoBehaviour
    {
        private BoardTileView[] _tiles;
        private bool _initialized;

        public void Initialize(int chunkSize, BoardTileView tilePrefab)
        {
            if (_initialized) return;

            int count = chunkSize * chunkSize;
            _tiles = new BoardTileView[count];
            for (int i = 0; i < count; i++)
            {
                _tiles[i] = Object.Instantiate(tilePrefab, transform);
                _tiles[i].gameObject.SetActive(false);
            }
            _initialized = true;
        }

        public void Setup(Vector2Int coord, BoardConfig config)
        {
            transform.position = ChunkCoordConverter.ChunkToWorld(coord, config);

            int chunkSize = config.chunkSize;
            for (int row = 0; row < chunkSize; row++)
            {
                for (int col = 0; col < chunkSize; col++)
                {
                    var tile = _tiles[row * chunkSize + col];
                    tile.transform.localPosition = new Vector3(col * config.tileSize.x, 0f, row * config.tileSize.y);
                    tile.MeshFilter.sharedMesh = (row + col) % 2 == 0 ? config.darkTile : config.lightTile;
                    tile.gameObject.SetActive(true);
                }
            }
        }

        public void Clear()
        {
            foreach (var tile in _tiles)
                tile.gameObject.SetActive(false);
        }
    }
}
