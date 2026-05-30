# Task 4: Edit Mode Tests

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Tests/Editor/EnemySpawnTests.cs`
- Create: `CB-client/Assets/Scripts/Tests/Editor/CrimsonBoard.Tests.EditMode.asmdef`

**Commit message:** `10 Add Edit Mode tests for spawn border tiles, weighted selection and wave transitions`

### Steps

1. **Create test folder** `CB-client/Assets/Scripts/Tests/Editor/` (two-level: Tests → Editor per project-structure.md convention).

2. **Create `CrimsonBoard.Tests.EditMode.asmdef`** in `Tests/Editor/`:
   ```json
   {
       "name": "CrimsonBoard.Tests.EditMode",
       "references": [
           "UnityEngine.TestRunner",
           "UnityEditor.TestRunner"
       ],
       "includePlatforms": [
           "Editor"
       ],
       "excludePlatforms": [],
       "allowUnsafeCode": false,
       "overrideReferences": true,
       "precompiledReferences": [
           "nunit.framework.dll"
       ],
       "autoReferenced": false,
       "defineConstraints": [],
       "versionDefines": [],
       "noEngineReferences": false
   }
   ```
   No explicit reference to `Assembly-CSharp` is needed: because `autoReferenced: false` and the main code has no asmdef, Unity implicitly links the default assembly when `overrideReferences` allows nunit access.

   > **Note:** If Unity reports unresolved `CrimsonBoard` namespace, add `"Assembly-CSharp"` to the `references` array.

3. **Create `EnemySpawnTests.cs`** in `Tests/Editor/`. Cover three scenarios:

   **A. Border tiles — correct perimeter for windowRadius=1, chunkSize=2**  
   `center=(0,0)`, `r=1`, `cs=2` → tile range `[-2..3, -2..3]`, 24 perimeter tiles.
   Test using a pure static helper (extract `GetBorderTiles` logic to a `public static` method `BorderTileHelper.Compute(Vector2Int center, int windowRadius, int chunkSize)` in `GameFieldSystem` or a dedicated static helper class in Core):
   ```csharp
   [Test]
   public void BorderTiles_WindowRadius1_ChunkSize2_Returns24Tiles()
   {
       var tiles = BorderTileHelper.Compute(Vector2Int.zero, windowRadius: 1, chunkSize: 2);
       Assert.AreEqual(24, tiles.Count);
   }

   [Test]
   public void BorderTiles_AllOnPerimeter()
   {
       var tiles = BorderTileHelper.Compute(Vector2Int.zero, windowRadius: 1, chunkSize: 2);
       // min=-2, max=3
       foreach (var t in tiles)
           Assert.IsTrue(t.x == -2 || t.x == 3 || t.y == -2 || t.y == 3,
               $"Tile {t} is not on perimeter");
   }
   ```
   To enable testing without Unity context, refactor the computation into a `public static` method `GameFieldSystem.ComputeBorderTiles(Vector2Int center, int windowRadius, int chunkSize)` that returns `List<Vector2Int>` and is called internally by `GetBorderTiles()`. Test the static method directly.

   **B. Weighted enemy selection — deterministic pick with known seed**  
   Create a minimal `WaveConfig` with two entries (weights 1.0 and 3.0). With seed=42 run 1000 picks; assert the heavier entry is chosen ~75% of the time (within ±10% tolerance).
   Extract `PickEnemyId` as a `public static` helper `SpawnHelper.PickEnemyId(EnemySpawnEntry[] entries, System.Random rng)`.
   ```csharp
   [Test]
   public void WeightedPick_HeavierEntryPickedMoreOften()
   {
       var entries = new[]
       {
           new EnemySpawnEntry { enemyId = 1, weight = 1f },
           new EnemySpawnEntry { enemyId = 2, weight = 3f },
       };
       var rng = new System.Random(42);
       int count2 = 0;
       for (int i = 0; i < 1000; i++)
           if (SpawnHelper.PickEnemyId(entries, rng) == 2) count2++;
       Assert.That(count2, Is.InRange(650, 850));
   }
   ```

   **C. Wave transition — index advances after waveInterval**  
   Since `EnemySpawnSystem.Tick` depends on `GameContext` (which requires Unity objects), test the wave timer logic through a lightweight stub OR extract wave advancement to a `public static` pure method. Alternatively, test the observable: after calling `Tick` N times with deltaTime such that total > `waveInterval`, verify `CurrentWaveIndex` increments.
   
   If full `EnemySpawnSystem` can't be unit-tested without Unity context, mark this test as `[UnityTest]` (Play Mode) and note it as a stretch goal. Add a simple Edit Mode test for the `waveInterval` boundary arithmetic instead:
   ```csharp
   [Test]
   public void WaveTimer_AdvancesAfterInterval()
   {
       // Pure arithmetic: after accumulating > interval, index increments
       float waveInterval = 30f;
       float timer = 0f;
       int waveIndex = 0;
       int maxWave = 2;
       float dt = 10f;
       for (int i = 0; i < 4; i++) // 4 * 10 = 40 > 30
       {
           timer += dt;
           if (waveIndex < maxWave && timer >= waveInterval)
           {
               timer -= waveInterval;
               waveIndex++;
           }
       }
       Assert.AreEqual(1, waveIndex);
       Assert.AreEqual(10f, timer, 0.001f);
   }
   ```

4. **Commit** both `.cs` and `.asmdef` files together with their `.meta` files (Unity will generate `.meta` on Editor open; commit them in a follow-up if needed, or note in PR).

## Implementation
**Status:** DONE
**Summary:** Создан test asmdef `CrimsonBoard.Tests.EditMode` и `EnemySpawnTests.cs` с 8 тестами: 4 на граничные тайлы (count, perimeter, non-zero center, no duplicates), 3 на взвешенный выбор врага, 2 на таймер волн.
