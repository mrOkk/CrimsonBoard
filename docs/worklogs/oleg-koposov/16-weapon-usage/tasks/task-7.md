# Task 7: Wire WeaponUsageSystem into GameplayState

Modify `States/GameplayState.cs`:

1. Create `WeaponUsageSystem` in `Enter()` and add to systems list
2. Call `Initialize()` on it
3. Include in `Tick()` loop
4. Call `Dispose()` in `Exit()`

The system should be created after player is spawned and inventory is initialized.

Files to modify: `States/GameplayState.cs`
