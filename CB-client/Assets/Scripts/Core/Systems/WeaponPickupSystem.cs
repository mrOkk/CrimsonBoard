using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class WeaponPickupSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly HashSet<WeaponView> _trackedWeapons = new();

        public WeaponPickupSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Board.WeaponDropped += OnWeaponDropped;
        }

        public void Tick(float deltaTime) { }

        public void Dispose()
        {
            _context.Board.WeaponDropped -= OnWeaponDropped;
            _trackedWeapons.Clear();
        }

        private void OnWeaponDropped(WeaponView weapon)
        {
            _trackedWeapons.Add(weapon);
        }

        public void TryPickupAt(Vector2Int cell)
        {
            var weapon = _context.TileMap.GetWeapon(cell);
            if (weapon == null) return;
            if (!_trackedWeapons.Contains(weapon)) return;

            var inventory = _context.Inventory;
            var weaponCfg = System.Array.Find(_context.Config.weapons, w => w.id == weapon.WeaponId);
            if (weaponCfg == null) return;

            bool isNew = inventory.TryAddWeapon(weapon.WeaponId);
            int ammo = weaponCfg.infiniteAmmo ? int.MaxValue : weaponCfg.ammoOnPickup;
            inventory.AddAmmo(weapon.WeaponId, ammo);

            if (isNew)
                inventory.SwitchTo(weapon.WeaponId);

            _trackedWeapons.Remove(weapon);
            _context.TileMap.UnregisterWeapon(cell);
            weapon.SetEquippedMode();
            _context.Pools.GetWeaponPool(weapon.WeaponId)?.Return(weapon);
        }
    }
}
