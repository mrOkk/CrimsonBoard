# Task 2: Wire pools into GameContext and InitState

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/GameContext.cs`
- Modify: `CB-client/Assets/Scripts/States/InitState.cs`

**Commit message:** 04 Wire GamePools into GameContext and InitState

### Steps

1. Добавить в `GameContext.cs` публичное свойство `public GamePools Pools { get; set; }`. Разместить рядом с `Config` (см. текущую структуру: поле `Config` задаётся через конструктор).

2. В `InitState.Enter()` создать `GamePools` и присвоить `_context.Pools`:
   ```csharp
   var pools = new GamePools(_context.Config.prefabs);
   _context.Pools = pools;
   ```
   Добавить `Debug.Log("[InitState] Pools initialized.");` после создания.
   Убрать комментарий `// TODO: load resources, init UI, load configs` и заменить на реальные действия.

## Implementation
<!-- Filled in Phase 3 -->
