# Task 1: Data layer — configs, HealthComponent, View wiring

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Configs/PlayerConfig.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/EnemyConfig.cs`
- Create: `CB-client/Assets/Scripts/Entities/HealthComponent.cs`
- Modify: `CB-client/Assets/Scripts/Entities/PlayerView.cs`
- Modify: `CB-client/Assets/Scripts/Entities/EnemyView.cs`

**Commit message:** `07 Add HealthComponent and wire into PlayerView and EnemyView`

### Steps

1. В `PlayerConfig.cs` изменить `public int health;` → `public float health;`.

2. В `EnemyConfig.cs` изменить `public int health;` → `public float health;` и `public int damage;` → `public float damage;`.

3. Создать `Entities/HealthComponent.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class HealthComponent : MonoBehaviour
       {
           private float _maxHp;
           private float _currentHp;

           public float CurrentHp => _currentHp;
           public float MaxHp => _maxHp;
           public bool IsDead => _currentHp <= 0f;

           public System.Action OnDeath;

           public void Init(float maxHp)
           {
               _maxHp = maxHp;
               _currentHp = maxHp;
           }

           public void TakeDamage(float amount)
           {
               if (IsDead) return;
               _currentHp = Mathf.Max(0f, _currentHp - amount);
               if (IsDead)
                   OnDeath?.Invoke();
           }

           public void Heal(float amount)
           {
               _currentHp = Mathf.Min(_maxHp, _currentHp + amount);
           }
       }
   }
   ```

4. В `PlayerView.cs` добавить `[SerializeField] private HealthComponent _health;` и публичное свойство `public HealthComponent Health => _health;`.

5. В `EnemyView.cs` добавить `[SerializeField] private HealthComponent _health;` и публичное свойство `public HealthComponent Health => _health;`. Добавить хранение конфига:
   ```csharp
   private EnemyConfig _config;
   public EnemyConfig Config => _config;

   public void Setup(EnemyConfig config)
   {
       _config = config;
       _health.Init(config.health);
   }
   ```

## Implementation
**Status:** DONE
**Summary:** `int health` → `float health` в PlayerConfig/EnemyConfig, `int damage` → `float damage` в EnemyConfig. Создан `HealthComponent` (MonoBehaviour). `PlayerView` получил `Health` property; `EnemyView` получил `Health` property и `Setup(EnemyConfig)` с инициализацией HP.
