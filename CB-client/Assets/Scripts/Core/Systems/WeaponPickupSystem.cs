using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class WeaponPickupSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly List<WeaponView> _dropped = new();

        public WeaponPickupSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize() { }
        public void Tick(float deltaTime) { }

        public void Dispose()
        {
            foreach (var w in _dropped)
                w.TriggerEntered = null;
            _dropped.Clear();
        }

        public void RegisterDropped(WeaponView weapon)
        {
            _dropped.Add(weapon);
            weapon.TriggerEntered += OnWeaponTrigger;
        }

        private void OnWeaponTrigger(WeaponView weapon, Collider other)
        {
            if (other.gameObject != _context.Player.gameObject) return;

            var inventory = _context.Inventory;
            var weaponCfg = System.Array.Find(_context.Config.weapons, w => w.id == weapon.WeaponId);
            if (weaponCfg == null) return;

            bool isNew = inventory.TryAddWeapon(weapon.WeaponId);
            int ammo = weaponCfg.infiniteAmmo ? int.MaxValue : weaponCfg.ammoOnPickup;
            inventory.AddAmmo(weapon.WeaponId, ammo);

            if (isNew)
                inventory.SwitchTo(weapon.WeaponId);

            weapon.TriggerEntered -= OnWeaponTrigger;
            _dropped.Remove(weapon);
            weapon.SetEquippedMode();
            _context.Pools.GetWeaponPool(weapon.WeaponId)?.Return(weapon);
        }
    }
}
