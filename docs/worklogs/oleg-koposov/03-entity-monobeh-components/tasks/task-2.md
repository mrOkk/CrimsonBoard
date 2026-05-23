# Task 2: Update PrefabsConfig

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs`

**Commit message:** `03 Replace GameObject with typed views in PrefabsConfig`

### Steps

1. Открыть `CB-client/Assets/Scripts/Core/Configs/PrefabsConfig.cs`.

2. Заменить поля `playerPrefab`, `enemyPrefab`, `weaponPrefab`, `powerUpPrefab`, `tilePrefab` с `GameObject` на соответствующие типы:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       [System.Serializable]
       public class PrefabsConfig
       {
           public PlayerView playerPrefab;
           public EnemyView enemyPrefab;
           public WeaponView weaponPrefab;
           public PowerUpView powerUpPrefab;
           public GameObject projectilePrefab;
           public BoardTileView tilePrefab;
       }
   }
   ```
   `projectilePrefab` остаётся `GameObject` — вне скопа этой задачи.

## Implementation

<!-- Filled in Phase 3 -->
