using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class GamePools
    {
        public ObjectPool<EnemyView> Enemies { get; }
        public IReadOnlyDictionary<int, ObjectPool<WeaponView>> Weapons { get; }
        public ObjectPool<ProjectileView> Projectiles { get; }
        public ObjectPool<PowerUpView> PowerUps { get; }

        public GamePools(PrefabsConfig prefabs)
        {
            Enemies = new ObjectPool<EnemyView>(prefabs.enemyPrefab, PoolConstants.Enemies);

            var weapons = new Dictionary<int, ObjectPool<WeaponView>>();
            foreach (var entry in prefabs.weaponPrefabs)
            {
                int id = entry.weaponId;
                weapons[id] = new ObjectPool<WeaponView>(
                    entry.prefab,
                    PoolConstants.WeaponsPerType,
                    w => w.SetWeaponId(id)
                );
            }
            Weapons = weapons;

            Projectiles = new ObjectPool<ProjectileView>(prefabs.projectilePrefab, PoolConstants.Projectiles);
            // PowerUps = new ObjectPool<PowerUpView>(prefabs.powerUpPrefab, PoolConstants.PowerUps);
        }

        public ObjectPool<WeaponView> GetWeaponPool(int weaponId)
        {
            if (Weapons.TryGetValue(weaponId, out var pool)) return pool;
            Debug.LogWarning($"[GamePools] No pool for weaponId={weaponId}");
            return null;
        }
    }
}
