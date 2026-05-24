using UnityEngine;

namespace CrimsonBoard
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "CrimsonBoard/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public PlayerConfig player;
        public EnemyConfig[] enemies;
        public WeaponConfig[] weapons;
        public TimingConfig timing;
        public BoardConfig board;
        public PrefabsConfig prefabs;
    }
}
