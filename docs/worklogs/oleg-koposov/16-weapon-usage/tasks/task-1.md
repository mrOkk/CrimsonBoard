# Task 1: Extend WeaponConfig

Add new fields to `Core/Configs/WeaponConfig.cs`:
- `float range` — дальность оружия (пуля уничтожается после прохождения)
- `float holsterTime = 0.3f` — время убирания оружия
- `float drawTime = 0.3f` — время доставания оружия
- `int maxTargetsPerBullet = 1` — пробивание (сколько врагов может пройти пуля)

Implementation:
```csharp
public float range = 10f;
public float holsterTime = 0.3f;
public float drawTime = 0.3f;
public int maxTargetsPerBullet = 1;
```
