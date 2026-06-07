using UnityEngine;

namespace CrimsonBoard
{
    public struct TileData
    {
        public EntityView Occupant;
        public WeaponView DroppedWeapon;

        public bool IsOccupied => Occupant != null;
        public bool HasWeapon => DroppedWeapon != null;

        public static TileData Empty => new TileData();

        public void Clear()
        {
            Occupant = null;
            DroppedWeapon = null;
        }
    }
}
