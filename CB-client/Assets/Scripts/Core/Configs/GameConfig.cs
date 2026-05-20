using UnityEngine;

namespace CrimsonBoard
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "CrimsonBoard/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public PlayerConfig player;
        public EnemyConfig enemy;
        public WeaponConfig weapon;
        public TimingConfig timing;
        public GameObject powerUpPrefab;
        public GameObject projectilePrefab;
    }
}
