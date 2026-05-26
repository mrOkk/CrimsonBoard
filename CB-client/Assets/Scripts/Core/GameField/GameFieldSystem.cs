using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class GameFieldSystem : IGameSystem
    {
        private readonly GameContext _context;
        private ObjectPool<ChunkView> _chunkPool;
        private readonly Dictionary<Vector2Int, ChunkView> _activeChunks = new Dictionary<Vector2Int, ChunkView>();
        private Vector2Int _currentCenter;

        public GameFieldSystem(GameContext context)
        {
            _context = context;
        }

        private bool _initialized;

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            int diameter = 2 * _context.Config.board.windowRadius + 1;
            int prewarm = diameter * diameter + 4;

            _chunkPool = new ObjectPool<ChunkView>(
                _context.Config.prefabs.chunkPrefab,
                prewarm,
                chunk => chunk.Initialize(_context.Config.board.chunkSize, _context.Config.prefabs.tilePrefab)
            );

            _currentCenter = Vector2Int.zero;
            LoadWindow(Vector2Int.zero);
        }

        public void Tick(float deltaTime) { }

        public void Dispose()
        {
            foreach (var kvp in _activeChunks)
            {
                kvp.Value.Clear();
                _chunkPool.Return(kvp.Value);
            }
            _activeChunks.Clear();
        }

        /// <summary>
        /// Call this when the player crosses into a new chunk.
        /// Integration point for the player movement system.
        /// </summary>
        public void OnPlayerChunkChanged(Vector2Int newCenter)
        {
            if (newCenter == _currentCenter) return;
            _currentCenter = newCenter;
            UpdateWindow(newCenter);
        }

        private void LoadWindow(Vector2Int center)
        {
            int r = _context.Config.board.windowRadius;
            for (int dx = -r; dx <= r; dx++)
                for (int dy = -r; dy <= r; dy++)
                    LoadChunk(center + new Vector2Int(dx, dy));
        }

        private void UpdateWindow(Vector2Int newCenter)
        {
            int r = _context.Config.board.windowRadius;
            var newCoords = new HashSet<Vector2Int>();
            for (int dx = -r; dx <= r; dx++)
                for (int dy = -r; dy <= r; dy++)
                    newCoords.Add(newCenter + new Vector2Int(dx, dy));

            var toUnload = new List<Vector2Int>();
            foreach (var coord in _activeChunks.Keys)
                if (!newCoords.Contains(coord))
                    toUnload.Add(coord);

            foreach (var coord in toUnload)
            {
                _activeChunks[coord].Clear();
                _chunkPool.Return(_activeChunks[coord]);
                _activeChunks.Remove(coord);
            }

            foreach (var coord in newCoords)
                if (!_activeChunks.ContainsKey(coord))
                    LoadChunk(coord);
        }

        private void LoadChunk(Vector2Int coord)
        {
            var chunk = _chunkPool.Get();
            chunk.Setup(coord, _context.Config.board);
            _activeChunks[coord] = chunk;
        }
    }
}
