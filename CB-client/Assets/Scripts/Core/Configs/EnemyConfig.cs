using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class EnemyConfig
    {
        public int id;
        public Mesh[] meshVariants;
        public int health;
        public int damage;
        public int movesPerBeat;
    }
}
