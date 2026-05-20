# Task 1: Config scripts

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Configs/PlayerConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/EnemyConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/WeaponConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/TimingConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/GameConfig.cs`

**Commit message:** `02 Add GameConfig ScriptableObject and config data structs`

### Steps

1. Создать папку `CB-client/Assets/Scripts/Core/Configs/`.

2. Создать `PlayerConfig.cs` — `[Serializable]` класс в пространстве имён `CrimsonBoard`:
   ```csharp
   [System.Serializable]
   public class PlayerConfig
   {
       public Mesh mesh;
       public GameObject prefab;
       public int health;
       public int movesPerBeat;
   }
   ```

3. Создать `EnemyConfig.cs`:
   ```csharp
   [System.Serializable]
   public class EnemyConfig
   {
       public Mesh[] meshVariants;
       public GameObject prefab;
       public int health;
       public int damage;
       public int movesPerBeat;
   }
   ```

4. Создать `WeaponConfig.cs`:
   ```csharp
   [System.Serializable]
   public class WeaponConfig
   {
       public Mesh[] meshVariants;
       public GameObject prefab;
       public int damage;
       public float shotsPerBeat;
       public float spread;
       public float rotationSpeed;
       public float moveSpeedCoefficient;
   }
   ```

5. Создать `TimingConfig.cs`:
   ```csharp
   [System.Serializable]
   public class TimingConfig
   {
       public float beatDuration;
       public float phaseDuration;
   }
   ```

6. Создать `GameConfig.cs` — `ScriptableObject` с `[CreateAssetMenu]`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       [CreateAssetMenu(fileName = "GameConfig", menuName = "CrimsonBoard/GameConfig")]
       public class GameConfig : ScriptableObject
       {
           public PlayerConfig player;
           public EnemyConfig enemy;
           public WeaponConfig weapon;
           public TimingConfig timing;
           public GameObject powerUpPrefab;
           public GameObject projectilePrefab;
       }
   }
   ```

7. Убедиться, что у всех `.cs`-файлов в папке `Configs/` пространство имён `CrimsonBoard` и все `using UnityEngine;` проставлены.

## Implementation

<!-- Filled in Phase 3 -->
