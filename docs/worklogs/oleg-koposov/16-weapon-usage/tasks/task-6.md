# Task 6: Update PlayerInputSystem

Modify `Core/Systems/PlayerInputSystem.cs`:

1. Add handling for Next/Previous weapon input:
   - In `Tick`, check `_input.Player.Next.WasPressedThisFrame()` and `_input.Player.Previous.WasPressedThisFrame()`
   - Call `_context.Inventory.CycleNext()` / `CyclePrevious()`

2. Delete the commented-out attack code in `TickShoot`:
   - Remove lines 102-106 (the `if (_input.Player.Attack.WasPressedThisFrame())` block)

Files to modify: `Core/Systems/PlayerInputSystem.cs`
