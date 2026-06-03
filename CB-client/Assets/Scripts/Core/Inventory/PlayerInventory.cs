using System.Collections.Generic;

namespace CrimsonBoard
{
    public class PlayerInventory
    {
        private readonly List<int> _weaponIds = new();
        private readonly Dictionary<int, int> _ammo = new();

        public IReadOnlyList<int> WeaponIds => _weaponIds;
        public int? ActiveWeaponId { get; private set; }

        public bool HasWeapon(int weaponId) => _weaponIds.Contains(weaponId);

        /// <returns>true if the weapon was newly added (not previously owned)</returns>
        public bool TryAddWeapon(int weaponId)
        {
            if (HasWeapon(weaponId)) return false;
            _weaponIds.Add(weaponId);
            if (ActiveWeaponId == null) ActiveWeaponId = weaponId;
            return true;
        }

        public void AddAmmo(int weaponId, int amount)
        {
            _ammo.TryGetValue(weaponId, out int current);
            _ammo[weaponId] = current + amount;
        }

        public int GetAmmo(int weaponId) =>
            _ammo.TryGetValue(weaponId, out int v) ? v : 0;

        public void SwitchTo(int weaponId)
        {
            if (HasWeapon(weaponId)) ActiveWeaponId = weaponId;
        }
    }
}
