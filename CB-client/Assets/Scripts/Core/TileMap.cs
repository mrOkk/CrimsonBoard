using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class TileMap
    {
        private readonly Dictionary<Vector2Int, TileData> _tiles = new Dictionary<Vector2Int, TileData>();

        public void RegisterEntity(Vector2Int cell, EntityView entity)
        {
            var data = GetOrCreate(cell);
            data.Occupant = entity;
            _tiles[cell] = data;
        }

        public void UnregisterEntity(Vector2Int cell)
        {
            if (_tiles.TryGetValue(cell, out var data))
            {
                data.Occupant = null;
                _tiles[cell] = data;
            }
        }

        public void RegisterWeapon(Vector2Int cell, WeaponView weapon)
        {
            var data = GetOrCreate(cell);
            data.DroppedWeapon = weapon;
            _tiles[cell] = data;
        }

        public void UnregisterWeapon(Vector2Int cell)
        {
            if (_tiles.TryGetValue(cell, out var data))
            {
                data.DroppedWeapon = null;
                _tiles[cell] = data;
            }
        }

        public bool IsOccupied(Vector2Int cell)
        {
            return _tiles.TryGetValue(cell, out var data) && data.Occupant != null;
        }

        public EntityView GetEntity(Vector2Int cell)
        {
            _tiles.TryGetValue(cell, out var data);
            return data.Occupant;
        }

        public WeaponView GetWeapon(Vector2Int cell)
        {
            _tiles.TryGetValue(cell, out var data);
            return data.DroppedWeapon;
        }

        public TileData GetTile(Vector2Int cell)
        {
            _tiles.TryGetValue(cell, out var data);
            return data;
        }

        public bool TryGetTile(Vector2Int cell, out TileData data)
        {
            return _tiles.TryGetValue(cell, out data);
        }

        public void ClearTile(Vector2Int cell)
        {
            if (_tiles.ContainsKey(cell))
            {
                var data = _tiles[cell];
                data.Clear();
                _tiles[cell] = data;
            }
        }

        private TileData GetOrCreate(Vector2Int cell)
        {
            return _tiles.TryGetValue(cell, out var data) ? data : TileData.Empty;
        }
    }
}
