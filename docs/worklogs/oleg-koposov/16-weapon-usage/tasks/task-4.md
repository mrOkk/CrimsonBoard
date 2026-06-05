# Task 4: Add Next/Previous to PlayerInventory

Add to `Core/Inventory/PlayerInventory.cs`:
- `int NextWeaponId` — property that returns next weapon ID in cycle, skipping weapons with 0 ammo (if not infinite)
- `int PreviousWeaponId` — same but in reverse cycle
- `void CycleNext()` — switches to NextWeaponId
- `void CyclePrevious()` — switches to PreviousWeaponId

Logic:
- Cycle through `_weaponIds` list
- Skip weapon if `!config.infiniteAmmo && GetAmmo(id) <= 0`
- If all weapons are empty, stay on current

Files to modify: `Core/Inventory/PlayerInventory.cs`
