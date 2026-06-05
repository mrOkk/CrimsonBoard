using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class GameBoard
    {
        public GameFieldSystem FieldSystem { get; }

        private readonly List<EnemyView> _activeEnemies = new();
        private readonly List<WeaponView> _droppedWeapons = new();

        public IReadOnlyList<EnemyView> ActiveEnemies => _activeEnemies;
        public IReadOnlyList<WeaponView> DroppedWeapons => _droppedWeapons;

        public GameBoard(GameFieldSystem fieldSystem)
        {
            FieldSystem = fieldSystem;
        }

        public List<Vector2Int> GetBorderTiles()
            => FieldSystem.GetBorderTiles();

        public void RegisterEnemy(EnemyView enemy) => _activeEnemies.Add(enemy);
        public void UnregisterEnemy(EnemyView enemy) => _activeEnemies.Remove(enemy);

        public void RegisterDroppedWeapon(WeaponView weapon) => _droppedWeapons.Add(weapon);
        public void UnregisterDroppedWeapon(WeaponView weapon) => _droppedWeapons.Remove(weapon);
    }
}
