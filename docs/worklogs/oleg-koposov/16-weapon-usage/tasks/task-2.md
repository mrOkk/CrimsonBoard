# Task 2: Add rotationPoint to WeaponView

Add to `Entities/WeaponView.cs`:
- `[SerializeField] private Transform _rotationPoint;`
- Public property `RotationPoint`

The rotationPoint is used as the pivot for holster/draw animation (rotation around X axis).
