using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class WaveConfig
    {
        public int maxAliveEnemies;
        public Vector2 spawnFrequencyRangeSec;   // x=min, y=max seconds between spawns
        public Vector2Int spawnBatchSizeRange;    // x=min, y=max enemies per batch
        public EnemySpawnEntry[] enemyTypes;
    }
}
