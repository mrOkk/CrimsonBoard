using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class BoardConfig
    {
        public Mesh darkTile;
        public Mesh lightTile;
        public Mesh sideTile;
        public Mesh cornerTile;
        public Vector2 tileSize;
        public Vector2Int boardSize;    // in tiles
    }
}

