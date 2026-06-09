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

        public void CycleNext()
        {
            if (_weaponIds.Count == 0) return;
            int startIdx = ActiveWeaponId.HasValue ? _weaponIds.IndexOf(ActiveWeaponId.Value) : 0;
            if (startIdx < 0) startIdx = 0;

            int idx = startIdx;
            for (int i = 0; i < _weaponIds.Count; i++)
            {
                idx = (idx + 1) % _weaponIds.Count;
                int wid = _weaponIds[idx];
                var cfg = GetWeaponConfig(wid);
                if (cfg != null && (cfg.infiniteAmmo || GetAmmo(wid) > 0))
                {
                    ActiveWeaponId = wid;
                    return;
                }
            }
        }

        public void CyclePrevious()
        {
            if (_weaponIds.Count == 0) return;
            int startIdx = ActiveWeaponId.HasValue ? _weaponIds.IndexOf(ActiveWeaponId.Value) : 0;
            if (startIdx < 0) startIdx = 0;

            int idx = startIdx;
            for (int i = 0; i < _weaponIds.Count; i++)
            {
                idx = (idx - 1 + _weaponIds.Count) % _weaponIds.Count;
                int wid = _weaponIds[idx];
                var cfg = GetWeaponConfig(wid);
                if (cfg != null && (cfg.infiniteAmmo || GetAmmo(wid) > 0))
                {
                    ActiveWeaponId = wid;
                    return;
                }
            }
        }

        private WeaponConfig GetWeaponConfig(int weaponId)
        {
            var ctx = GameContext.Instance;
            if (ctx == null || ctx.Config == null)
            {
                return null;
            }

            for (var index = 0; index < ctx.Config.weapons.Length; index++)
            {
                var w = ctx.Config.weapons[index];

                if (w.id == weaponId)
                {
                    return w;
                }
            }

            return null;
        }
    }
}
