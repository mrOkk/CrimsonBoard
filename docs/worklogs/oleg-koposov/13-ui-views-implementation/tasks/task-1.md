# Task 1: Add GameStats and wire into GameContext

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/GameStats.cs`
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`

**Commit message:** `13 Add GameStats and wire into GameContext`

### Steps

1. **Create `GameStats.cs`** — plain C# class in namespace `CrimsonBoard`:
   ```csharp
   public class GameStats
   {
       public int Score { get; private set; }
       public float ElapsedBattleTime { get; private set; }

       public void Reset()
       {
           Score = 0;
           ElapsedBattleTime = 0f;
       }

       public void AddScore(int amount) => Score += amount;

       public void Tick(float deltaTime) => ElapsedBattleTime += deltaTime;
   }
   ```

2. **Modify `GameContext.cs`** — add property and initialize in constructor:
   - Add property: `public GameStats Stats { get; } = new GameStats();`
   - Place after `UiRoot UiRoot` property (precedent: UiRoot was added after GameFieldSystem).

## Implementation
**Status:** DONE
**Summary:** Created `GameStats.cs` с полями Score и ElapsedBattleTime; добавлено `Stats` property в `GameContext` с инициализацией при объявлении.
