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
        public EnemyType enemyType;
        public int rank;              // higher rank = harder to override; used by Knight collision rules
        public int moveCooldownTicks; // beats to wait between moves (0 = move every beat)
    }
}
