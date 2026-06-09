using UnityEngine;
using Object = UnityEngine.Object;

namespace CrimsonBoard
{
    public class TileMap
    {
        private readonly GameConfig _gameConfig;
        private readonly TileData[,] _tilesArray;
        private readonly int[] _shuffledIndices;
        private bool _isSpawned;
        private Transform _root;

        public Vector2Int PlayerCell => GameContext.Instance.Player.CurrentCell;

        public TileMap(GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            var boardConfig = gameConfig.board;
            _tilesArray = new TileData[boardConfig.boardSize.x, boardConfig.boardSize.y];
            _shuffledIndices = new int[boardConfig.boardSize.x * boardConfig.boardSize.y];
            for (int i = 0; i < _shuffledIndices.Length; i++)
            {
                _shuffledIndices[i] = i;
            }
        }

        public void Spawn()
        {
            if (_isSpawned)
            {
                return;
            }

            _isSpawned = true;
            _root = new GameObject("TileMap").transform;

            for (int x = 0; x < _tilesArray.GetLength(0); x++)
            {
                var isOddRow = (x % 2 == 0);
                var oddMesh = isOddRow ? _gameConfig.board.darkTile : _gameConfig.board.lightTile;
                var evenMesh = isOddRow ? _gameConfig.board.lightTile : _gameConfig.board.darkTile;

                for (int y = 0; y < _tilesArray.GetLength(1); y++)
                {
                    var tileView = Object.Instantiate(
                        _gameConfig.prefabs.tilePrefab,
                        new Vector3(x * _gameConfig.board.tileSize.x, 0, y * _gameConfig.board.tileSize.y),
                        Quaternion.identity,
                        _root);
                    tileView.MeshFilter.sharedMesh = (y % 2 == 0) ? evenMesh : oddMesh;
                    _tilesArray[x, y].TileView = tileView;
                }
            }

            // TODO: combine meshes for better performance
        }

        public void RegisterEntity(Vector2Int cell, EntityView entity)
        {
            ref var data = ref GetTile(cell);
            data.Occupant = entity;
        }

        public void UnregisterEntity(Vector2Int cell)
        {
            ref var data = ref GetTile(cell);
            data.Occupant = null;
        }

        public void RegisterWeapon(Vector2Int cell, WeaponView weapon)
        {
            ref var data = ref GetTile(cell);
            data.DroppedWeapon = weapon;
        }

        public void UnregisterWeapon(Vector2Int cell)
        {
            ref var data = ref GetTile(cell);
            data.DroppedWeapon = null;
        }

        public bool IsOccupied(Vector2Int cell)
        {
            return _tilesArray[cell.x, cell.y].IsOccupied;
        }

        public EntityView GetEntity(Vector2Int cell)
        {
            return _tilesArray[cell.x, cell.y].Occupant;
        }

        public WeaponView GetWeapon(Vector2Int cell)
        {
            return _tilesArray[cell.x, cell.y].DroppedWeapon;
        }

        public void ClearTile(Vector2Int cell)
        {
            ref var data = ref GetTile(cell);
            data.Clear();
        }

        public ref TileData GetTile(Vector2Int cell)
        {
            return ref _tilesArray[cell.x, cell.y];
        }

        public bool IsValidCell(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < _tilesArray.GetLength(0) && cell.y >= 0 && cell.y < _tilesArray.GetLength(1);
        }

        public int[] GetShuffledIndexes()
        {
            _shuffledIndices.Shuffle(GameContext.Instance.SharedRandom);
            return _shuffledIndices;
        }

        public Vector2Int IndexToCell(int index)
        {
            var width = _tilesArray.GetLength(0);
            return new Vector2Int(index % width, index / width);
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(cell.x * _gameConfig.board.tileSize.x, 0, cell.y * _gameConfig.board.tileSize.y);
        }
    }
}
