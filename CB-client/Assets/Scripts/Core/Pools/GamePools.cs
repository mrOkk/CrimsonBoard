namespace CrimsonBoard
{
    public class GamePools
    {
        public ObjectPool<EnemyView> Enemies { get; }
        public ObjectPool<WeaponView> Weapons { get; }
        public ObjectPool<ProjectileView> Projectiles { get; }
        public ObjectPool<PowerUpView> PowerUps { get; }

        public GamePools(PrefabsConfig prefabs)
        {
            Enemies = new ObjectPool<EnemyView>(prefabs.enemyPrefab, PoolConstants.Enemies);
            Weapons = new ObjectPool<WeaponView>(prefabs.weaponPrefab, PoolConstants.Weapons);
            Projectiles = new ObjectPool<ProjectileView>(prefabs.projectilePrefab, PoolConstants.Projectiles);
            PowerUps = new ObjectPool<PowerUpView>(prefabs.powerUpPrefab, PoolConstants.PowerUps);
        }
    }
}
