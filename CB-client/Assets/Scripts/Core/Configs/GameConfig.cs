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
        public HopConfig hop;
        public SpawnConfig spawn;

#if UNITY_EDITOR
        private static GameConfig _editorInstance;

        public static GameConfig EditorInstance
        {
            get
            {
                if (_editorInstance != null) return _editorInstance;
                var guids = UnityEditor.AssetDatabase.FindAssets("t:GameConfig");
                if (guids.Length == 0) return null;
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                _editorInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<GameConfig>(path);
                return _editorInstance;
            }
        }
#endif
    }
}
