# Task 3: Implement ProjectileView

Implement `Entities/ProjectileView.cs`:
- Fields: `Rigidbody _rb`, `float _speed`, `int _pierceLeft`, `float _damage`, `float _range`
- Method `Launch(dir, speed, damage, pierceCount, range)`:
  - Set velocity in direction
  - Store pierce count and range
  - Return to pool after `range / speed` seconds
- `OnTriggerEnter`:
  - If hit `EnemyView`: apply damage via `HealthComponent.TakeDamage`
  - Decrease `_pierceLeft`, if 0 return to pool
- `OnTriggerEnter` should ignore triggers that are not enemies

Implementation notes:
- Use `GetComponent<EnemyView>()` or `GetComponentInParent<EnemyView>()` to detect enemy
- Use `GetComponent<HealthComponent>()` to apply damage
- Return via `_context.Pools.Projectiles.Return(this)`
- Needs reference to `GameContext` (via static `Instance`)

Files to modify: `Entities/ProjectileView.cs`
