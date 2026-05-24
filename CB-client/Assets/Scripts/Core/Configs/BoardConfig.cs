using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class BoardConfig
    {
        public Mesh darkTile;
        public Mesh lightTile;
        public Vector2 tileSize;
        public int chunkSize = 16;    // tiles per chunk side
        public int windowRadius = 1;  // active window = (2*windowRadius+1)^2 chunks
    }
}

