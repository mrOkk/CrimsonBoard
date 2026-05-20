using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class EnemyConfig
    {
        public Mesh[] meshVariants;
        public GameObject prefab;
        public int health;
        public int damage;
        public int movesPerBeat;
    }
}
