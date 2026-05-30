# Task 1: Create UI infrastructure (BaseView + UiRoot)

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/UI/BaseView.cs`
- Create: `CB-client/Assets/Scripts/UI/UiRoot.cs`

**Commit message:** `12 Add BaseView and UiRoot UI infrastructure`

### Steps

1. **Create folder** `CB-client/Assets/Scripts/UI/` (new subfolder inside the existing `Scripts/` tree that uses `CB-client.asmdef`).

2. **Create `BaseView.cs`** — abstract MonoBehaviour in namespace `CrimsonBoard`:
   ```csharp
   namespace CrimsonBoard
   {
       public abstract class BaseView : MonoBehaviour
       {
           public virtual void Show() => gameObject.SetActive(true);
           public virtual void Hide() => gameObject.SetActive(false);
           public virtual void Tick(float deltaTime) { }
       }
   }
   ```

3. **Create `UiRoot.cs`** — MonoBehaviour in namespace `CrimsonBoard`:
   - Field: `private Dictionary<Type, BaseView> _views`
   - `Init()`: iterate `GetComponentsInChildren<BaseView>(true)`, register each by `view.GetType()` — called explicitly from `EntryPoint`, not in `Awake`
   - `Show<T>()` where `T : BaseView`: look up type, call `view.Show()`
   - `Hide<T>()` where `T : BaseView`: look up type, call `view.Hide()`
   - `GetView<T>()` where `T : BaseView`: return typed view or log warning + return null
   - `Tick(float deltaTime)`: iterate all registered views and call `view.Tick(deltaTime)` — precedent: `GameplaySystemRunner.Tick` in `Gameplay/GameplaySystemRunner.cs`

## Implementation
**Status:** DONE
**Summary:** Created `Scripts/UI/BaseView.cs` (abstract MonoBehaviour with Show/Hide/Tick) and `Scripts/UI/UiRoot.cs` (MonoBehaviour facade with Init, Show\<T\>/Hide\<T\>/GetView\<T\>/Tick). No deviations from plan.
