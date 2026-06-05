using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class PrefabsConfig
    {
        public PlayerView playerPrefab;
        public EnemyView enemyPrefab;
        public WeaponPrefabEntry[] weaponPrefabs;
        public PowerUpView powerUpPrefab;
        public ProjectileView projectilePrefab;
        public BoardTileView tilePrefab;
        public ChunkView chunkPrefab;
        public HitEmitter hitEmitterPrefab;
    }
}
