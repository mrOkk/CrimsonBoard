using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class PrefabsConfig
    {
        public PlayerView playerPrefab;
        public EnemyView enemyPrefab;
        public WeaponView weaponPrefab;
        public PowerUpView powerUpPrefab;
        public ProjectileView projectilePrefab;
        public BoardTileView tilePrefab;
    }
}
