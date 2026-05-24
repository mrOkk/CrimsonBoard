# Task 1: Foundation — tile coords, EntityView.CurrentCell, PlayerView.DirectionIndicator

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/GameField/ChunkCoordConverter.cs`
- Modify: `CB-client/Assets/Scripts/Entities/EntityView.cs`
- Modify: `CB-client/Assets/Scripts/Entities/PlayerView.cs`

**Commit message:** `08 Add tile coord helpers, CurrentCell and DirectionIndicator`

### Steps

1. В `ChunkCoordConverter.cs` добавить два статических метода после `ChunkToWorld`:
   ```csharp
   /// <summary>Converts a world position to tile grid coordinates.</summary>
   public static Vector2Int WorldToTile(Vector3 worldPos, BoardConfig config)
   {
       return new Vector2Int(
           Mathf.FloorToInt(worldPos.x / config.tileSize.x),
           Mathf.FloorToInt(worldPos.z / config.tileSize.y)
       );
   }

   /// <summary>Returns the world-space centre of a tile cell.</summary>
   public static Vector3 TileToWorld(Vector2Int cell, BoardConfig config)
   {
       return new Vector3(
           (cell.x + 0.5f) * config.tileSize.x,
           0f,
           (cell.y + 0.5f) * config.tileSize.y
       );
   }
   ```

2. В `EntityView.cs` добавить публичное поле (не SerializeField — выставляется системами):
   ```csharp
   public Vector2Int CurrentCell { get; set; }
   ```

3. В `PlayerView.cs` добавить `[SerializeField] private Transform _directionIndicator;` и свойство `public Transform DirectionIndicator => _directionIndicator;`.

## Implementation
<!-- Filled in Phase 3 -->
