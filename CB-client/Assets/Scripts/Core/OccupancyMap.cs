using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class OccupancyMap
    {
        private readonly Dictionary<Vector2Int, EntityView> _cells = new Dictionary<Vector2Int, EntityView>();

        public void Register(Vector2Int cell, EntityView entity) => _cells[cell] = entity;

        public void Unregister(Vector2Int cell) => _cells.Remove(cell);

        public bool IsOccupied(Vector2Int cell) => _cells.ContainsKey(cell);

        public EntityView GetEntity(Vector2Int cell)
        {
            _cells.TryGetValue(cell, out var entity);
            return entity;
        }
    }
}
