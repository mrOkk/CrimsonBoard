using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public int enemyId;
        [Min(0f)] public float weight;
    }
}
