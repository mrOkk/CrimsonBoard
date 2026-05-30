# Task 1: Spawn config classes

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Configs/EnemySpawnEntry.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/WaveConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/SpawnConfig.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/GameConfig.cs`

**Commit message:** `10 Add spawn wave config classes`

### Steps

1. **Create `EnemySpawnEntry.cs`** in `Core/Configs/`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       [System.Serializable]
       public class EnemySpawnEntry
       {
           public int enemyId;
           [Min(0f)] public float weight;
       }
   }
   ```
   Pattern: identical structure to `EnemyConfig` (`[System.Serializable]`, namespace `CrimsonBoard`).

2. **Create `WaveConfig.cs`** in `Core/Configs/`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       [System.Serializable]
       public class WaveConfig
       {
           public int maxAliveEnemies;
           public Vector2 spawnFrequencyRangeSec;   // x=min, y=max seconds between spawns
           public Vector2Int spawnBatchSizeRange;    // x=min, y=max enemies per batch
           public EnemySpawnEntry[] enemyTypes;
       }
   }
   ```

3. **Create `SpawnConfig.cs`** in `Core/Configs/`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       [System.Serializable]
       public class SpawnConfig
       {
           public float waveInterval;               // seconds between wave promotions
           public int randomSeed;                   // seed for deterministic spawn RNG
           public WaveConfig[] waves;
       }
   }
   ```

4. **Modify `GameConfig.cs`** — add `public SpawnConfig spawn;` field after `hop`:
   ```csharp
   public HopConfig hop;
   public SpawnConfig spawn;
   ```
   Existing fields must remain in the same order to preserve serialized asset data.

5. Create `.meta` files: Unity will auto-generate them on next Editor open; **no manual action needed** — just commit the `.cs` files.

## Implementation
**Status:** DONE
**Summary:** Созданы `EnemySpawnEntry`, `WaveConfig`, `SpawnConfig` как `[Serializable]` классы. В `GameConfig` добавлено поле `spawn` после `hop`.
