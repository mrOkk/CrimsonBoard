using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class EnemyConfig
    {
        public int id;
        public Mesh mesh;
        public float health;
        public float damage;
        public int movesPerBeat;
    }
}
