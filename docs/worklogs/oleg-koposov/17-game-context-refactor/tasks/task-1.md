# Task 1: Create GameBoard data class

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/GameBoard.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`

**Commit message:** 17 create GameBoard data class

### Steps

1. Создать `CB-client/Assets/Scripts/Core/GameBoard.cs`:
   ```csharp
   using System.Collections.Generic;
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class GameBoard
       {
           public GameFieldSystem FieldSystem { get; }

           private readonly List<EnemyView> _activeEnemies = new();
           private readonly List<WeaponView> _droppedWeapons = new();

           public IReadOnlyList<EnemyView> ActiveEnemies => _activeEnemies;
           public IReadOnlyList<WeaponView> DroppedWeapons => _droppedWeapons;

           public GameBoard(GameFieldSystem fieldSystem)
           {
               FieldSystem = fieldSystem;
           }

           public List<Vector2Int> GetBorderTiles()
               => FieldSystem.GetBorderTiles();

           public void RegisterEnemy(EnemyView enemy) => _activeEnemies.Add(enemy);
           public void UnregisterEnemy(EnemyView enemy) => _activeEnemies.Remove(enemy);

           public void RegisterDroppedWeapon(WeaponView weapon) => _droppedWeapons.Add(weapon);
           public void UnregisterDroppedWeapon(WeaponView weapon) => _droppedWeapons.Remove(weapon);
       }
   }
   ```

2. Добавить `public GameBoard Board { get; set; }` в `GameContext`.

3. Создать `.meta`-файл через Unity при первом коммите (Unity Editor сгенерирует автоматически; для CLI можно оставить как есть — Unity создаст при следующем open project).

## Implementation
**Status:** DONE
**Summary:** Created GameBoard class with ActiveEnemies, DroppedWeapons lists and methods. Added Board property to GameContext.
