namespace CrimsonBoard
{
    [System.Serializable]
    public class WeaponPrefabEntry
    {
        [WeaponId]
        public int weaponId;
        public WeaponView prefab;
    }
}
