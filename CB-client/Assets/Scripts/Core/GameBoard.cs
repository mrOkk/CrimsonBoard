using System.Collections.Generic;

namespace CrimsonBoard
{
    public class GameBoard
    {
        private readonly List<EnemyView> _activeEnemies = new();

        public IReadOnlyList<EnemyView> ActiveEnemies => _activeEnemies;

        public event System.Action<WeaponView> WeaponDropped;

        public void RegisterEnemy(EnemyView enemy) => _activeEnemies.Add(enemy);
        public void UnregisterEnemy(EnemyView enemy) => _activeEnemies.Remove(enemy);

        public void NotifyWeaponDropped(WeaponView weapon) => WeaponDropped?.Invoke(weapon);
    }
}
