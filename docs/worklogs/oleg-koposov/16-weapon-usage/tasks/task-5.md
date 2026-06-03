# Task 5: Create WeaponUsageSystem

Create `Core/Systems/WeaponUsageSystem.cs` implementing `IGameSystem`.

Responsibilities:
1. Track weapon instances attached to player
2. Handle weapon switching with holster/draw animation
3. Rotate player towards nearest enemy
4. Fire projectiles on beat timing

### Fields
- `GameContext _context`
- `Dictionary<int, WeaponView> _equippedWeapons` — weaponId → instance
- `int? _activeWeaponId`
- `bool _isSwitching`
- `float _shotTimer`

### Initialize
- Subscribe to `PlayerInventory.ActiveWeaponId` changes (or poll in Tick)
- Populate `_equippedWeapons` from `PlayerInventory.WeaponIds` by instantiating from pool

### Tick
- Update active weapon visibility (show active, hide others)
- Handle weapon switch animation (coroutine or state machine)
- Find nearest enemy in range
- Rotate player towards enemy if not moving
- Fire shot if timer elapsed and player is stationary

### Key Methods
- `AttachWeapon(weaponId)` — get from pool, parent to `weaponLocator`, add to dict
- `DetachWeapon(weaponId)` — return to pool, remove from dict
- `StartSwitch(newWeaponId)` — begin holster animation, then draw
- `HolsterAnimation(callback)` — rotate weapon down around rotationPoint X axis
- `DrawAnimation(callback)` — rotate weapon up from down position
- `TryFireShot()` — spawn projectile from muzzle, reset timer based on `shotsPerBeat`
- `FindNearestEnemyInRange()` — iterate active enemies, check distance

### Animation Details
- Holster: rotate `WeaponView.transform` by -90° around X axis at `rotationPoint.position`
- Draw: rotate back to 0°
- Duration from `WeaponConfig.holsterTime` / `drawTime`
- Use `Coroutine` for animation

Files to create: `Core/Systems/WeaponUsageSystem.cs`
