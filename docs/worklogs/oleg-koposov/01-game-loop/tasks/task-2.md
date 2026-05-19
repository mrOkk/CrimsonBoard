# Task 2: Core interfaces — IGameState and IGameSystem

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Core/IGameState.cs` + `.meta`
- Create: `CB-client/Assets/Scripts/Core/IGameSystem.cs` + `.meta`

**Commit message:** `01 Add IGameState and IGameSystem interfaces`

### Steps

1. Create `IGameState.cs` in `CB-client/Assets/Scripts/Core/`:
   ```csharp
   namespace CrimsonBoard
   {
       public interface IGameState
       {
           void Enter();
           void Exit();
           void Tick(float deltaTime);
       }
   }
   ```

2. Create `IGameSystem.cs` in `CB-client/Assets/Scripts/Core/`:
   ```csharp
   namespace CrimsonBoard
   {
       public interface IGameSystem
       {
           void Initialize();
           void Tick(float deltaTime);
           void Dispose();
       }
   }
   ```

3. Create `.meta` files for both `.cs` files using the MonoImporter template:
   ```yaml
   fileFormatVersion: 2
   guid: <unique-guid>
   MonoImporter:
     externalObjects: {}
     serializedVersion: 2
     defaultReferences: []
     executionOrder: 0
     icon: {instanceID: 0}
     userData: 
     assetBundleName: 
     assetBundleVariant: 
   ```

4. Commit all new files.

## Implementation
**Status:** DONE
**Summary:** Created IGameState.cs (Enter/Exit/Tick) and IGameSystem.cs (Initialize/Tick/Dispose) with paired .meta files.
