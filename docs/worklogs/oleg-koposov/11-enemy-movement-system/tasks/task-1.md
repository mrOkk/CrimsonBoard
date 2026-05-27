# Task 1: EnemyConfig extensions

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/Configs/EnemyType.cs`
- Modify: `CB-client/Assets/Scripts/Core/Configs/EnemyConfig.cs`

**Commit message:** `11 Add EnemyType enum and extend EnemyConfig with type, rank, moveCooldownTicks`

### Steps

1. **Create `EnemyType.cs`** in `Core/Configs/`:
   ```csharp
   namespace CrimsonBoard
   {
       public enum EnemyType
       {
           Pawn,
           Knight,
           Rook,    // moves diagonally up to 5 cells
           Tower,   // moves straight up to 5 cells
           Queen,   // moves in all 8 directions up to 6 cells
       }
   }
   ```

2. **Modify `EnemyConfig.cs`** — add three fields after `movesPerBeat`:
   ```csharp
   public EnemyType enemyType;
   public int rank;              // higher rank = harder to override; used by Knight collision rules
   public int moveCooldownTicks; // beats to wait between moves (0 = move every beat)
   ```
   Existing fields (`id`, `mesh`, `health`, `damage`, `movesPerBeat`) must remain in order to preserve serialized asset data.

## Implementation
**Status:** DONE
**Summary:** Created `EnemyType.cs` enum with 5 values (Pawn/Knight/Rook/Tower/Queen); extended `EnemyConfig` with `enemyType`, `rank`, and `moveCooldownTicks` fields after existing fields to preserve serialized data.
