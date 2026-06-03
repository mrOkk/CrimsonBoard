namespace CrimsonBoard
{
    [System.Serializable]
    public class InventoryEntry
    {
        [WeaponId]
        public int weaponId;
        public int ammoCount;
    }
}
